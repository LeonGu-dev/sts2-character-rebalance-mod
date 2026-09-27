using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DefectRebalance.DefectRebalanceCode.Hooks;

/// <summary>
/// 临时存储:每个吞噬暗影能力最近一次"未被格挡的球伤害"。
/// 黑暗球激发的伤害结算(AfterDamageGiven)先写入,随后 AfterOrbEvoked 读取使用。
/// </summary>
public sealed class UnblockedDamageHolder
{
    public decimal Value;
}

public static class ConsumingShadowCapture
{
    public static readonly ConditionalWeakTable<ConsumingShadowPower, UnblockedDamageHolder> Captured = new();
}

/// <summary>
/// 平衡调整:吞噬暗影(ConsumingShadow)重做。
/// 原:2费稀有(能力),充2(升3)颗黑暗球,回合末能力激发最右球。
/// 新:2费稀有(能力),不再充球;你激发黑暗充能球后,获得其未被格挡伤害的 50%(未强化)/100%(强化) 作为格挡。
/// </summary>

[HarmonyPatch(typeof(ConsumingShadow), "CanonicalVars", MethodType.Getter)]
public static class ConsumingShadowVarsPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
        => new DynamicVar[] { new PowerVar<ConsumingShadowPower>(50m) };
}

[HarmonyPatch(typeof(ConsumingShadow), "OnUpgrade")]
public static class ConsumingShadowUpgradePatch
{
    public static bool Prefix(ConsumingShadow __instance)
    {
        __instance.DynamicVars["ConsumingShadowPower"].UpgradeValueBy(50m);
        return false;
    }
}

[HarmonyPatch(typeof(ConsumingShadow), "OnPlay")]
public static class ConsumingShadowOnPlayPatch
{
    public static bool Prefix() => false;

    public static async Task Postfix(Task __result, ConsumingShadow __instance, PlayerChoiceContext choiceContext)
    {
        if (__result != null)
        {
            await __result;
        }
        await CreatureCmd.TriggerAnim(__instance.Owner!.Creature, "PowerUp", __instance.Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<ConsumingShadowPower>(choiceContext,
            __instance.Owner.Creature, __instance.DynamicVars["ConsumingShadowPower"].BaseValue,
            __instance.Owner.Creature, __instance);
    }
}

/// <summary>吞噬暗影旧效果移除:回合末不再激发最右球。
/// 注意:AfterSideTurnEnd 返回 Task,仅 Prefix()=>false 会让方法返回 null,调用方 await null 抛 NRE。
/// 必须补一个返回 Task.CompletedTask 的 pass-through postfix。</summary>
[HarmonyPatch(typeof(ConsumingShadowPower), "AfterSideTurnEnd")]
public static class ConsumingShadowOldPowerPatch
{
    public static bool Prefix() => false;

    public static Task Postfix(Task __result) => Task.CompletedTask;
}

/// <summary>捕获能力拥有者打出的"无牌源、非能力加成"球类伤害的未格挡值。</summary>
[HarmonyPatch(typeof(AbstractModel), "AfterDamageGiven")]
public static class ConsumingShadowDamageCapturePatch
{
    public static void Postfix(AbstractModel __instance, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (__instance is not ConsumingShadowPower p)
        {
            return;
        }
        if (dealer != p.Owner || cardSource != null || target.IsPlayer)
        {
            return;
        }
        if (!props.HasFlag(ValueProp.Unpowered))
        {
            return;
        }
        ConsumingShadowCapture.Captured.AddOrUpdate(p, new UnblockedDamageHolder { Value = result.UnblockedDamage });
    }
}

/// <summary>激发黑暗充能球后,按 50%/100% × 未被格挡伤害 获得格挡。</summary>
[HarmonyPatch(typeof(AbstractModel), "AfterOrbEvoked")]
public static class ConsumingShadowBlockPatch
{
    public static async Task Postfix(Task __result, AbstractModel __instance, PlayerChoiceContext choiceContext, OrbModel orb, IEnumerable<Creature> targets)
    {
        if (__instance is not ConsumingShadowPower p || orb is not DarkOrb || orb.Owner != p.Owner.Player)
        {
            return;
        }
        decimal unblocked = ConsumingShadowCapture.Captured.TryGetValue(p, out UnblockedDamageHolder? holder) ? holder.Value : 0m;
        ConsumingShadowCapture.Captured.Remove(p);
        if (unblocked > 0m)
        {
            await CreatureCmd.GainBlock(p.Owner, p.Amount / 100m * unblocked, ValueProp.Unpowered, null);
        }
    }
}

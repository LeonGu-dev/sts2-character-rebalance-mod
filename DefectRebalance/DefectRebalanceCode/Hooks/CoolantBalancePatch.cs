using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Orbs;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace DefectRebalance.DefectRebalanceCode.Hooks;

/// <summary>
/// 冷却剂生成的"临时冰霜球"登记表(回合开始时直接移除、不激发)。
/// </summary>
public static class CoolantTempOrbRegistry
{
    public static readonly ConditionalWeakTable<OrbModel, object> TempFrostOrbs = new();
}

/// <summary>
/// 平衡调整:冷却剂(Coolant)重做。
/// 原:1费稀有(能力),回合开始按球种数×2获得格挡;升级能力+1。
/// 新:1费稀有(能力),打出攻击牌后获得1颗临时冰霜球(回合末正常触发被动,下回合开始直接消失不激发);升级获得【固有】。
/// </summary>

[HarmonyPatch(typeof(Coolant), "OnUpgrade")]
public static class CoolantUpgradePatch
{
    public static bool Prefix(Coolant __instance)
    {
        __instance.AddKeyword(CardKeyword.Innate);
        return false;
    }
}

/// <summary>冷却剂旧效果移除:回合开始不再按球种数给格挡。
/// 注意:AfterSideTurnStart 返回 Task,仅 Prefix()=>false 会让方法返回 null,调用方 await null 抛 NRE。
/// 必须补一个返回 Task.CompletedTask 的 pass-through postfix。</summary>
[HarmonyPatch(typeof(CoolantPower), "AfterSideTurnStart")]
public static class CoolantOldPowerPatch
{
    public static bool Prefix() => false;

    public static Task Postfix(Task __result) => Task.CompletedTask;
}

/// <summary>打出攻击牌后,充一颗冰霜球并登记为临时球。
/// 注意:必须先登记再 Channel——Channel 内部会立即创建球视觉并调用 UpdateVisuals,
/// 若登记在 Channel 之后,光晕 patch 查不到登记。</summary>
[HarmonyPatch(typeof(AbstractModel), "AfterCardPlayed")]
public static class CoolantChannelPatch
{
    public static async Task Postfix(Task __result, AbstractModel __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (__instance is not CoolantPower p || cardPlay.Card.Type != CardType.Attack || cardPlay.Player != p.Owner.Player)
        {
            return;
        }
        OrbModel orb = ModelDb.Orb<FrostOrb>().ToMutable();
        CoolantTempOrbRegistry.TempFrostOrbs.AddOrUpdate(orb, new object());
        await OrbCmd.Channel(choiceContext, orb, p.Owner.Player);
    }
}

/// <summary>回合开始:直接把登记的临时冰霜球移出队列(不触发激发),并同步移除栏位显示。</summary>
[HarmonyPatch(typeof(OrbQueue), "AfterTurnStart")]
public static class CoolantTempOrbRemovalPatch
{
    public static void Prefix(OrbQueue __instance)
    {
        foreach (OrbModel orb in __instance.Orbs.Where(o => CoolantTempOrbRegistry.TempFrostOrbs.TryGetValue(o, out _)).ToList())
        {
            __instance.Remove(orb);
            NCombatRoom.Instance?.GetCreatureNode(orb.Owner?.Creature)?.OrbManager?.EvokeOrbAnim(orb);
            CoolantTempOrbRegistry.TempFrostOrbs.Remove(orb);
        }
    }
}

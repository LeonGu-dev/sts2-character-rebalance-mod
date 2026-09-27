using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace DefectRebalance.DefectRebalanceCode.Hooks;

/// <summary>
/// 平衡调整:冰之长枪(IceLance)重做。
/// 原:3费稀有攻击,造成19(升24)伤害+充3颗冰霜球。
/// 新:1费罕见攻击,激发所有冰霜充能球,每激发一颗对随机敌人造成6(升9)点伤害(替换原冰霜Evoke格挡)。
/// </summary>

[HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
public static class IceLanceRarityPatch
{
    public static CardRarity Postfix(CardRarity __result, CardModel __instance)
        => __instance is IceLance ? CardRarity.Uncommon : __result;
}

[HarmonyPatch(typeof(CardModel), "TargetType", MethodType.Getter)]
public static class IceLanceTargetTypePatch
{
    public static TargetType Postfix(TargetType __result, CardModel __instance)
        => __instance is IceLance ? TargetType.RandomEnemy : __result;
}

[HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
public static class IceLanceCostPatch
{
    public static int Postfix(int __result, CardModel __instance)
        => __instance is IceLance ? 1 : __result;
}

[HarmonyPatch(typeof(IceLance), "CanonicalVars", MethodType.Getter)]
public static class IceLanceVarsPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
        => new DynamicVar[] { new DamageVar(6m, ValueProp.Move) };
}

[HarmonyPatch(typeof(IceLance), "OnUpgrade")]
public static class IceLanceUpgradePatch
{
    public static bool Prefix(IceLance __instance)
    {
        __instance.DynamicVars["Damage"].UpgradeValueBy(3m);
        return false;
    }
}

[HarmonyPatch(typeof(IceLance), "OnPlay")]
public static class IceLanceOnPlayPatch
{
    public static bool Prefix() => false;

    public static async Task Postfix(Task __result, IceLance __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (__result != null)
        {
            await __result;
        }
        var owner = __instance.Owner!;
        var queue = owner.PlayerCombatState!.OrbQueue;
        foreach (OrbModel orb in queue.Orbs.OfType<FrostOrb>().ToList())
        {
            bool removed = queue.Remove(orb);
            NCombatRoom.Instance?.GetCreatureNode(owner.Creature)?.OrbManager?.EvokeOrbAnim(orb);
            choiceContext.PushModel(orb);
            IEnumerable<Creature> targets = await orb.Evoke(choiceContext);
            await DamageCmd.Attack(__instance.DynamicVars.Damage.BaseValue).FromCard(__instance, cardPlay)
                .TargetingRandomOpponents(owner.Creature.CombatState!)
                .Execute(choiceContext);
            choiceContext.PopModel(orb);
            if (owner.Creature.CombatState != null)
            {
                await Hook.AfterOrbEvoked(choiceContext, owner.Creature.CombatState!, orb, targets);
                if (removed)
                {
                    orb.RemoveInternal();
                }
            }
        }
    }
}

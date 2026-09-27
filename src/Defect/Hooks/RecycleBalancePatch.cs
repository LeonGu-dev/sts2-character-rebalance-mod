using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace DefectRebalance.DefectRebalanceCode.Hooks;

/// <summary>
/// 平衡调整:内存清理(Scavenge)重做为"回收"(Recycle)。
/// 原:1费罕见技能,消耗1张牌,下回合获得2(升3)能量。
/// 新:1费(升级0)罕见技能,消耗一张牌,获得与其当前费用相等的能量。
/// 名称与描述通过本地化覆盖(SCAVENGE.title/.description)改为"回收"。
/// </summary>

[HarmonyPatch(typeof(Scavenge), "CanonicalVars", MethodType.Getter)]
public static class ScavengeVarsPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
        => System.Array.Empty<DynamicVar>();
}

[HarmonyPatch(typeof(Scavenge), "ExtraHoverTips", MethodType.Getter)]
public static class ScavengeHoverTipPatch
{
    public static IEnumerable<IHoverTip> Postfix(IEnumerable<IHoverTip> __result)
        => new IHoverTip[] { HoverTipFactory.FromKeyword(CardKeyword.Exhaust) };
}

[HarmonyPatch(typeof(Scavenge), "OnUpgrade")]
public static class ScavengeUpgradePatch
{
    public static bool Prefix(Scavenge __instance)
    {
        __instance.EnergyCost.UpgradeBy(-1);
        return false;
    }
}

[HarmonyPatch(typeof(Scavenge), "OnPlay")]
public static class ScavengeOnPlayPatch
{
    public static bool Prefix() => false;

    public static async Task Postfix(Task __result, Scavenge __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (__result != null)
        {
            await __result;
        }
        CardModel? card = (await CardSelectCmd.FromHand(choiceContext, __instance.Owner!,
            new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1), null, __instance)).FirstOrDefault();
        if (card == null)
        {
            return;
        }
        int cost = card.EnergyCost.CostsX
            ? card.EnergyCost.GetAmountToSpend()
            : card.EnergyCost.GetWithModifiers(CostModifiers.All);
        await CardCmd.Exhaust(choiceContext, card);
        if (cost > 0)
        {
            await PlayerCmd.GainEnergy(cost, __instance.Owner!);
        }
    }
}

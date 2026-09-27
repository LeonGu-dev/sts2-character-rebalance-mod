using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace DefectRebalance.DefectRebalanceCode.Hooks;

/// <summary>
/// 平衡调整:衍生牌"燃料"(Fuel,由压缩 Compact 生成)。
/// 原效果:0费token,获得1(升2)能量。
/// 新效果:改为抽1(升2)张牌。
/// </summary>

[HarmonyPatch(typeof(Fuel), "CanonicalVars", MethodType.Getter)]
public static class FuelVarsPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
        => new DynamicVar[] { new CardsVar(1) };
}

[HarmonyPatch(typeof(Fuel), "ExtraHoverTips", MethodType.Getter)]
public static class FuelHoverTipPatch
{
    public static IEnumerable<IHoverTip> Postfix(IEnumerable<IHoverTip> __result)
        => Enumerable.Empty<IHoverTip>();
}

[HarmonyPatch(typeof(Fuel), "OnUpgrade")]
public static class FuelOnUpgradePatch
{
    public static bool Prefix(Fuel __instance)
    {
        __instance.DynamicVars.Cards.UpgradeValueBy(1m);
        return false;
    }
}

[HarmonyPatch(typeof(Fuel), "OnPlay")]
public static class FuelOnPlayPatch
{
    public static bool Prefix() => false;

    public static async Task Postfix(Task __result, Fuel __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (__result != null)
        {
            await __result;
        }
        await CardPileCmd.Draw(choiceContext, __instance.DynamicVars.Cards.BaseValue, __instance.Owner);
    }
}

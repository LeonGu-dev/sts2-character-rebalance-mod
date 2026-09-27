using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Cards;

namespace DefectRebalance.DefectRebalanceCode.Hooks;

/// <summary>
/// 平衡调整:模组改造(Modded)的费用惩罚由"本局+1"改为"本回合+1"。
/// 原效果:打出后 +1 球槽、抽1(升2)张,然后本局费用+1(AddThisCombat)。
/// 新效果:打出后本回合费用+1(AddThisTurn),机制与战士"战鼓"一致。
/// 实现:原 OnPlay 已 AddThisCombat(1),postfix 抵消该值并改为 AddThisTurn(1)。
/// </summary>
[HarmonyPatch(typeof(Modded), "OnPlay")]
public static class ModdedOnPlayPatch
{
    public static async Task Postfix(Task __result, Modded __instance)
    {
        await __result;
        __instance.EnergyCost.AddThisCombat(-1);
        __instance.EnergyCost.AddThisTurn(1);
    }
}

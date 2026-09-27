using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace DefectRebalance.DefectRebalanceCode.Hooks;

/// <summary>
/// 平衡调整:碎片整理(Defragment)稀有度由稀有(Rare)降低为罕见(Uncommon)。
/// 效果不变:集中+1/2。Defragment 未重写 Rarity(构造传入),故 patch 基类 getter 按类型分发。
/// </summary>
[HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
public static class DefragmentRarityPatch
{
    public static CardRarity Postfix(CardRarity __result, CardModel __instance)
        => __instance is Defragment ? CardRarity.Uncommon : __result;
}

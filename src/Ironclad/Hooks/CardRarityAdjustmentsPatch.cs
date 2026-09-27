using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MySts2ContentMod.MySts2ContentModCode.Hooks;

/// <summary>
/// 平衡调整(稀有度调整汇总):部分卡片稀有度被调整。
/// 由于这些卡片自身没有 override Rarity 属性(Rarity 继承自 CardModel),
/// 无法通过 typeof(具体卡) patch 其 getter,故在此 patch 基类
/// CardModel.Rarity 的 getter,按实例类型返回调整后的稀有度。
/// 涉及:
///   巨像(Colossus):Uncommon → Common
///   黑暗之拥(DarkEmbrace):Rare → Uncommon
///   巩固(Entrench):Event → Uncommon
/// </summary>
[HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
public static class CardRarityAdjustmentsPatch
{
    public static CardRarity Postfix(CardRarity __result, CardModel __instance)
    {
        return __instance switch
        {
            Colossus => CardRarity.Common,
            DarkEmbrace => CardRarity.Uncommon,
            Entrench => CardRarity.Uncommon,
            _ => __result
        };
    }
}
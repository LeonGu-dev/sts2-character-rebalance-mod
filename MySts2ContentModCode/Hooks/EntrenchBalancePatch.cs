using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MySts2ContentMod.MySts2ContentModCode.Hooks;

/// <summary>
/// 平衡调整(巩固 Entrench):将巩固加入铁甲战士卡池(IroncladCardPool),
/// 使其能在战斗后的正常卡牌奖励中与普通 Uncommon 卡一样出现。
/// 原版巩固注册在 EventCardPool(事件池)且稀有度为 Event,只通过事件获得;
/// 普通卡牌奖励只从角色自身卡池(player.Character.CardPool)抽取,
/// 因此仅修改稀有度(见 CardRarityAdjustmentsPatch)不会让它进入奖励。
/// </summary>
[HarmonyPatch(typeof(IroncladCardPool), "GenerateAllCards")]
public static class EntrenchPoolPatch
{
    public static CardModel[] Postfix(CardModel[] __result)
    {
        return __result.Append(ModelDb.Card<Entrench>()).ToArray();
    }
}

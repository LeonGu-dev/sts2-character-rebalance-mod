using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MySts2ContentMod.MySts2ContentModCode.Hooks;

/// <summary>
/// 卡图修复(巩固 Entrench):巩固只有 event/beta 变体卡图,
/// 正常卡图路径(atlases/card_atlas.sprites/event/entrench.tres)在游戏中不存在,
/// 导致以普通卡展示(卡牌奖励/图鉴/手牌)时无卡图。
/// 此处为巩固返回其事件卡使用的 beta 卡图。
/// </summary>
[HarmonyPatch(typeof(CardModel), "Portrait", MethodType.Getter)]
public static class EntrenchPortraitPatch
{
    private const string EntrenchBetaPortraitPath = "res://images/atlases/card_atlas.sprites/event/beta/entrench.tres";

    public static Texture2D Postfix(Texture2D __result, CardModel __instance)
    {
        if (__instance is Entrench)
        {
            return ResourceLoader.Load<Texture2D>(EntrenchBetaPortraitPath, null, ResourceLoader.CacheMode.Reuse);
        }
        return __result;
    }
}

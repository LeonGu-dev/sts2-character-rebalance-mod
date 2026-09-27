using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MySts2ContentMod.MySts2ContentModCode.Hooks;

/// <summary>
/// 平衡调整:烙印(Brand)力量成长由 1/2 改为 1/1,强化后可以抽 1 张牌。
/// 原效果:失去 1 点生命,消耗 1 张牌,获得 1 点力量(强化后 2)。
/// 新效果:失去 1 点生命,消耗 1 张牌,获得 1 点力量(强化后仍为 1);
///         强化后额外抽 1 张牌。
///
/// Brand 是基类 sealed 卡牌,用补丁改写 OnUpgrade(不再增加力量)与 OnPlay(强化时抽 1)。
/// </summary>
[HarmonyPatch(typeof(Brand), "OnUpgrade")]
public static class BrandOnUpgradePatch
{
    public static bool Prefix(Brand __instance)
    {
        return false;
    }
}

/// <summary>
/// 烙印(Brand)OnPlay 后置补丁:强化后额外抽 1 张牌。
/// 原 OnPlay 已完成后(消耗牌、获得力量),若为强化版则补抽 1 张。
/// </summary>
[HarmonyPatch(typeof(Brand), "OnPlay")]
public static class BrandOnPlayPatch
{
    public static async Task Postfix(Task __result, Brand __instance, PlayerChoiceContext choiceContext)
    {
        await __result;
        if (__instance.IsUpgraded)
        {
            await CardPileCmd.Draw(choiceContext, 1m, __instance.Owner);
        }
    }
}
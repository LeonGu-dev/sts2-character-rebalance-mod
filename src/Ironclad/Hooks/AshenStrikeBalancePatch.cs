using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MySts2ContentMod.MySts2ContentModCode.Hooks;

/// <summary>
/// 平衡调整:灰烬打击(AshenStrike)伤害成长由 3/4 降低为 2/3。
/// 原效果:消耗堆中每有一张牌,额外伤害 +3(强化后 +4)。
/// 新效果:消耗堆中每有一张牌,额外伤害 +2(强化后 +3)。
/// 总伤害 = 6 基础伤害 + (额外伤害 × 消耗堆张数)。
///
/// AshenStrike 是基类 sealed 卡牌,无法继承改写,因此用 Harmony 补丁替换
/// CanonicalVars 中的 ExtraDamageVar 基值(3 → 2);升级逻辑仍为 +1,故得到 2/3。
/// </summary>
[HarmonyPatch(typeof(AshenStrike), "CanonicalVars", MethodType.Getter)]
public static class AshenStrikeBalancePatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
    {
        return __result.Select(v => v is ExtraDamageVar ? new ExtraDamageVar(2m) : v);
    }
}
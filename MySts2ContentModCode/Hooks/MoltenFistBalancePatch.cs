using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace MySts2ContentMod.MySts2ContentModCode.Hooks;

/// <summary>
/// 平衡调整:熔融之拳(MoltenFist)基础伤害由 10/14 降低为 8/12。
/// 原效果:10 伤害(强化后 14),目标易伤翻倍。
/// 新效果:8 伤害(强化后 12),目标易伤翻倍(不变)。
/// 仅替换 CanonicalVars 中的 DamageVar 基值(10 → 8);升级仍 +4,故得到 8/12。
/// </summary>
[HarmonyPatch(typeof(MoltenFist), "CanonicalVars", MethodType.Getter)]
public static class MoltenFistBalancePatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
    {
        return __result.Select(v => v is DamageVar damage ? new DamageVar(8m, ValueProp.Move) : v);
    }
}
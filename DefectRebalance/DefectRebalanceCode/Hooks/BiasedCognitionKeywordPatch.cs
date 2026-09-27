using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace DefectRebalance.DefectRebalanceCode.Hooks;

/// <summary>
/// 平衡调整:偏差认知(BiasedCognition)新增关键词【固有】【保留】。
/// 效果不变:集中+5/6,附聚焦衰减。BiasedCognition 未重写 CanonicalKeywords,故 patch 基类 getter。
/// </summary>
[HarmonyPatch(typeof(CardModel), "CanonicalKeywords", MethodType.Getter)]
public static class BiasedCognitionKeywordPatch
{
    public static IEnumerable<CardKeyword> Postfix(IEnumerable<CardKeyword> __result, CardModel __instance)
        => __instance is BiasedCognition
            ? new[] { CardKeyword.Innate, CardKeyword.Retain }.Concat(__result ?? Enumerable.Empty<CardKeyword>())
            : __result;
}

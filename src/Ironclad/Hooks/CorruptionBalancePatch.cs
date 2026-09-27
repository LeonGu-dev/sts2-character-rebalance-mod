using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MySts2ContentMod.MySts2ContentModCode.Hooks;

/// <summary>
/// 平衡调整:腐化(Corruption)升级效果由「费用 -1(3→2)」改为「获得[固有]」。
/// 原效果:3 费能力,本场战斗中打出技能牌时改为消耗(并保留 3 费)。
/// 新效果:3 费能力,效果不变;强化后获得[固有],不再降低费用。
/// </summary>
[HarmonyPatch(typeof(Corruption), "OnUpgrade")]
public static class CorruptionOnUpgradePatch
{
    public static bool Prefix(Corruption __instance)
    {
        __instance.AddKeyword(CardKeyword.Innate);
        return false;
    }
}
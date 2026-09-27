using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MySts2ContentMod.MySts2ContentModCode.Hooks;

/// <summary>
/// 平衡调整:与我一战(FightMe)力量由 3/4 降低为 2/3,且升级后不再增加伤害。
/// 原效果:5 伤害 × 2,自身 +3 力量(强化后 +4),敌人 +1 力量;升级同时增加伤害(+1)。
/// 新效果:5 伤害 × 2,自身 +2 力量(强化后 +3),敌人 +1 力量;升级只增加力量,不再增加伤害。
/// </summary>
[HarmonyPatch(typeof(FightMe), "CanonicalVars", MethodType.Getter)]
public static class FightMeBalancePatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
    {
        return __result.Select(v => v is PowerVar<StrengthPower> strength ? new PowerVar<StrengthPower>(2m) : v);
    }
}

/// <summary>
/// 与我一战(FightMe)升级逻辑:原升级为伤害 +1 与力量 +1,现改为仅力量 +1。
/// </summary>
[HarmonyPatch(typeof(FightMe), "OnUpgrade")]
public static class FightMeOnUpgradePatch
{
    public static bool Prefix(FightMe __instance)
    {
        __instance.DynamicVars.Strength.UpgradeValueBy(1m);
        return false;
    }
}

/// <summary>
/// 与我一战(FightMe)OnPlay:改为先获得力量,再造成伤害,最后给敌人力量。
/// 注意:Harmony 此版本不支持 async prefix,故用「同步 skip prefix + async postfix」替换 OnPlay。
/// </summary>
[HarmonyPatch(typeof(FightMe), "OnPlay")]
public static class FightMeOnPlayPatch
{
    public static bool Prefix()
    {
        return false;
    }

    public static async Task Postfix(Task __result, FightMe __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (__result != null)
        {
            await __result;
        }

        await PowerCmd.Apply<StrengthPower>(choiceContext, __instance.Owner.Creature,
            __instance.DynamicVars["StrengthPower"].BaseValue, __instance.Owner.Creature, __instance);
        await DamageCmd.Attack(__instance.DynamicVars.Damage.BaseValue)
            .WithHitCount(__instance.DynamicVars.Repeat.IntValue)
            .FromCard(__instance, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);
        await PowerCmd.Apply<StrengthPower>(choiceContext, cardPlay.Target,
            __instance.DynamicVars["EnemyStrength"].BaseValue, __instance.Owner.Creature, __instance);
    }
}
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MySts2ContentMod.MySts2ContentModCode.Hooks;

/// <summary>
/// 平衡调整(改升级):绯红披风(CrimsonMantle)。
/// 原:1费能力,每回合开始时失去1点生命(每次打出+1)并获得7格挡;升级后格挡+3。
/// 新:效果不变;升级后不再失去生命(取消失去生命的副作用),格挡保持7(升级不再+3)。
/// 实现:OnUpgrade 改为空操作(不 +3 格挡);OnPlay 升级版不再调用 IncrementSelfDamage。
/// </summary>
[HarmonyPatch(typeof(CrimsonMantle), "OnUpgrade")]
public static class CrimsonMantleUpgradePatch
{
    public static bool Prefix()
    {
        return false;
    }
}

/// <summary>
/// 绯红披风(CrimsonMantle)OnPlay:升级版不再增加自伤(不失去生命)。
/// 原 OnPlay 在 Apply power 后调用 IncrementSelfDamage()。
/// 注意:Harmony 此版本不支持 async prefix,故用「同步 skip prefix + async postfix」。
/// 未升级:走原逻辑(prefix 返回 true)。升级:替换(应用 power 但不 IncrementSelfDamage)。
/// </summary>
[HarmonyPatch(typeof(CrimsonMantle), "OnPlay")]
public static class CrimsonMantleOnPlayPatch
{
    public static bool Prefix(CrimsonMantle __instance)
    {
        return !__instance.IsUpgraded;
    }

    public static async Task Postfix(Task __result, CrimsonMantle __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (__instance.IsUpgraded)
        {
            await PowerCmd.Apply<CrimsonMantlePower>(choiceContext, __instance.Owner.Creature,
                __instance.DynamicVars["CrimsonMantlePower"].BaseValue, __instance.Owner.Creature, __instance);
        }
        else if (__result != null)
        {
            await __result;
        }
    }
}
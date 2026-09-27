using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MySts2ContentMod.MySts2ContentModCode.Hooks;

/// <summary>
/// 平衡调整(改机制):战鼓(DrumOfBattle)。
/// 原:1费技能,抽2张;这张牌被消耗时,获得2能量(升级+1);升级能量+1。
/// 新:1费技能,抽2张;每次打出这张牌,本回合其费用+1;升级:费用降为0。
///     移除"被消耗时获得能量"的效果。
/// </summary>
[HarmonyPatch(typeof(DrumOfBattle), "OnPlay")]
public static class DrumOfBattleOnPlayPatch
{
    public static async Task Postfix(Task __result, DrumOfBattle __instance)
    {
        await __result;
        __instance.EnergyCost.AddThisTurn(1);
    }
}

/// <summary>
/// 战鼓(DrumOfBattle)移除原效果:被消耗时获得能量。
/// 注意:Harmony 此版本在 prefix 返回 false 跳过 async Task 方法时,会返回 null Task,
/// 而 Hook.AfterCardExhausted 会直接 await 该 Task,导致 NullReferenceException。
/// 因此必须配合 async postfix 返回已完成的 Task。
/// </summary>
[HarmonyPatch(typeof(DrumOfBattle), "AfterCardExhausted")]
public static class DrumOfBattleExhaustPatch
{
    public static bool Prefix()
    {
        return false;
    }

    public static async Task Postfix(Task __result)
    {
        if (__result != null)
        {
            await __result;
        }
    }
}

/// <summary>
/// 战鼓(DrumOfBattle)升级:原能量+1,改为费用 1→0。
/// </summary>
[HarmonyPatch(typeof(DrumOfBattle), "OnUpgrade")]
public static class DrumOfBattleUpgradePatch
{
    public static bool Prefix(DrumOfBattle __instance)
    {
        __instance.EnergyCost.UpgradeBy(-1);
        return false;
    }
}
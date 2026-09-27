using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MySts2ContentMod.MySts2ContentModCode.Hooks;

/// <summary>
/// 平衡调整:好勇斗狠(Aggression)升级效果由「固有」改为「0 费」。
/// 原效果:回合开始时,将弃牌堆中一张随机攻击牌放入手牌并升级;升级后获得「固有」。
/// 新效果:同上;升级后费用降低为 0(1 费 → 0 费),不再获得「固有」。
///
/// Aggression 是基类 sealed 卡牌,补丁 OnUpgrade 使其不再加固有、改为降低费用。
/// </summary>
[HarmonyPatch(typeof(Aggression), "OnUpgrade")]
public static class AggressionBalancePatch
{
    public static bool Prefix(Aggression __instance)
    {
        __instance.EnergyCost.UpgradeBy(-1);
        return false;
    }
}
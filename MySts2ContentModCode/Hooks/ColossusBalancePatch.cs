using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MySts2ContentMod.MySts2ContentModCode.Hooks;

/// <summary>
/// 平衡调整:巨像(Colossus)格挡由 4/7 提高为 6/9(升级仍 +3),稀有度由 Uncommon 降为 Common。
/// 原效果:获得 4 点格挡(强化后 7);本回合有易伤的敌人对你伤害降低 50%。
/// 新效果:获得 6 点格挡(强化后 9);有易伤的敌人对你伤害降低 25%。
/// </summary>
[HarmonyPatch(typeof(Colossus), "CanonicalVars", MethodType.Getter)]
public static class ColossusBlockPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
    {
        return __result.Select(v => v is BlockVar block ? new BlockVar(6m, block.Props) : v);
    }
}

/// <summary>
/// 巨像之力(ColossusPower)减伤效果:由 50% 降低为 25%。
/// 该 Power 仅由巨像(Colossus)卡牌使用,修改其 DamageDecrease 变量即可。
/// 注意:DamageDecrease 是伤害倍率(0.75 = 降低25%),而非减免量。
/// </summary>
[HarmonyPatch(typeof(ColossusPower), "CanonicalVars", MethodType.Getter)]
public static class ColossusPowerPatch
{
    public static IEnumerable<DynamicVar> Postfix(IEnumerable<DynamicVar> __result)
    {
        return __result.Select(v => v.Name == "DamageDecrease" ? new DynamicVar("DamageDecrease", 0.75m) : v);
    }
}
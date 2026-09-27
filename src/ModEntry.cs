using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace RebalanceMod;

[ModInitializer(nameof(Initialize))]
public partial class ModEntry : Node
{
    public const string ModId = "RebalanceMod";
    public const string IroncladResources = "res://RebalanceMod/IroncladRebalance";
    public const string SilentResources = "res://RebalanceMod/SilentRebalance";
    public const string DefectResources = "res://RebalanceMod/DefectRebalance";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();
    }
}

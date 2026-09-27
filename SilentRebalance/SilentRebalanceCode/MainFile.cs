using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace SilentRebalance.SilentRebalanceCode;

#if !MERGED_REBALANCE
[ModInitializer(nameof(Initialize))]
#endif
public partial class MainFile : Node
{
    public const string ModId = "SilentRebalance"; //Used for resource filepath
#if MERGED_REBALANCE
    public const string ResPath = "res://RebalanceMod/SilentRebalance";
#else
    public const string ResPath = $"res://{ModId}";
#endif

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

#if !MERGED_REBALANCE
    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();
    }
#endif
}

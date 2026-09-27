using Godot;

namespace RebalanceMod;

/// <summary>Resolves character artwork inside the single RebalanceMod resource pack.</summary>
public static class AssetPaths
{
    public static string Card(string characterRoot, string fileName, bool big = false)
        => Resolve(characterRoot, big ? "card_portraits/big" : "card_portraits", fileName, "card.png");

    public static string Power(string characterRoot, string fileName, bool big = false)
        => Resolve(characterRoot, big ? "powers/big" : "powers", fileName, "power.png");

    private static string Resolve(string characterRoot, string folder, string fileName, string fallback)
    {
        string directory = $"{characterRoot}/images/{folder}";
        string candidate = $"{directory}/{fileName}";
        if (ResourceLoader.Exists(candidate))
        {
            return candidate;
        }

        ModEntry.Logger.Info($"Could not find image path: {candidate}");
        return $"{directory}/{fallback}";
    }
}

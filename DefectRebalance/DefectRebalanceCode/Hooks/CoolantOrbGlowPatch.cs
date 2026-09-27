using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Orbs;

namespace DefectRebalance.DefectRebalanceCode.Hooks;

/// <summary>
/// 视觉增强:冷却剂生成的"临时冰霜球"在栏位中加一圈粉红色光晕,正常充能球不受影响。
/// 挂在 NOrb.UpdateVisuals 上,仅当 NOrb.Model 已在 CoolantTempOrbRegistry 登记(即冷却剂临时球)时,
/// 在球的 Spine 骨架后方动态添加一个径向渐变光晕节点。
/// </summary>
[HarmonyPatch(typeof(NOrb), "UpdateVisuals")]
public static class CoolantOrbGlowPatch
{
    private const string GlowNodeName = "CoolantGlow";

    private static readonly FieldInfo SpriteField = AccessTools.Field(typeof(NOrb), "_sprite");

    public static void Postfix(NOrb __instance)
    {
        bool isTemp = __instance.Model != null
            && CoolantTempOrbRegistry.TempFrostOrbs.TryGetValue(__instance.Model, out _);
        object? spriteObj = SpriteField.GetValue(__instance);
        bool isNode2D = spriteObj is Node2D;
        bool already = isNode2D && ((Node2D)spriteObj).GetNodeOrNull(GlowNodeName) != null;
        GD.Print($"[CoolantOrbGlow] Postfix Model={__instance.Model?.Id} isTemp={isTemp} sprite={spriteObj?.GetType().Name} isNode2D={isNode2D} already={already}");
        if (!isTemp)
        {
            return;
        }
        if (!isNode2D || already)
        {
            GD.Print("[CoolantOrbGlow] skip: sprite not Node2D or glow already exists");
            return;
        }
        ((Node2D)spriteObj).AddChild(CreateGlow());
        GD.Print($"[CoolantOrbGlow] glow added to {((Node)spriteObj).Name}");
    }

    private static Sprite2D CreateGlow()
    {
        Gradient gradient = new Gradient();
        gradient.SetColor(0, new Color(1f, 0.4f, 0.7f, 0.85f));
        gradient.SetColor(1, new Color(1f, 0.3f, 0.6f, 0f));

        GradientTexture2D texture = new GradientTexture2D
        {
            Gradient = gradient,
            Width = 128,
            Height = 128,
            Fill = GradientTexture2D.FillEnum.Radial,
            FillFrom = new Vector2(0.5f, 0.5f),
            FillTo = new Vector2(0.5f, 0f)
        };

        return new Sprite2D
        {
            Name = GlowNodeName,
            Texture = texture,
            Centered = true,
            ZIndex = 100,
            Scale = new Vector2(0.62f, 0.62f)
        };
    }
}

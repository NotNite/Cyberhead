using HarmonyLib;
using Reptile;
using UnityEngine;

namespace Cyberhead.Patches;

[HarmonyPatch(typeof(CharacterVisual))]
public static class CharacterVisualPatch {
    [HarmonyPrefix]
    [HarmonyPatch("SetBoostpackEffect")]
    public static bool SetBoostpackEffect(
        CharacterVisual __instance,
        BoostpackEffectMode set, float overrideScale = -1f
    ) {
        if (!Plugin.CyberheadConfig.General.VrEnabled.Value) return true;

        var player = __instance.transform.parent.parent.gameObject;
        var mainPlayer = WorldHandler.instance.GetCurrentPlayer().gameObject;
        if (player == mainPlayer) {
            __instance.VFX.boostpackEffect.SetActive(false);
            __instance.VFX.boostpackBlueEffect.SetActive(false);
            __instance.VFX.boostpackTrail.SetActive(false);
            return false;
        }
        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch("SetMoveStyleVisualAnim")]
    public static void SetMoveStyleVisualAnim(
        CharacterVisual __instance,
        Player player,
        MoveStyle setMoveStyle,
        GameObject specialSkateboard = null
    ) {
        Plugin.ApplyIk(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch("SetPhone")]
    public static void SetPhone(CharacterVisual __instance, bool set) {
        Plugin.ApplyIk(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch("SetSpraycan")]
    public static void SetSpraycan(CharacterVisual __instance, bool set, Characters c = Characters.NONE) {
        Plugin.ApplyIk(__instance);
    }
}

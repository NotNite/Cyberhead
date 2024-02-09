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

        var player = __instance.transform.parent;
        if (player == null) return true;
        player = player.parent;
        if (player == null) return true;

        var worldHandler = WorldHandler.instance;
        if (worldHandler == null) return true;
        var currentPlayer = worldHandler.GetCurrentPlayer();
        if (currentPlayer == null) return true;

        if (player.gameObject == currentPlayer.gameObject) {
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
        Utils.ApplyIk(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch("SetPhone")]
    public static void SetPhone(CharacterVisual __instance, bool set) {
        Utils.ApplyIk(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch("SetSpraycan")]
    public static void SetSpraycan(CharacterVisual __instance, bool set, Characters c = Characters.NONE) {
        Utils.ApplyIk(__instance);
    }
}

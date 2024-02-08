using HarmonyLib;
using Reptile;

namespace Cyberhead.Patches;

[HarmonyPatch(typeof(CharacterVisual))]
public static class CharacterVisualPatch {
    [HarmonyPrefix]
    [HarmonyPatch("SetBoostpackEffect")]
    public static bool SetBoostpackEffect(
        CharacterVisual __instance,
        BoostpackEffectMode set, float overrideScale = -1f
    ) {
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
}

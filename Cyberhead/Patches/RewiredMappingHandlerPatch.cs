using HarmonyLib;
using Reptile;
using Rewired;

namespace Cyberhead.Patches;

[HarmonyPatch(typeof(RewiredMappingHandler))]
public class RewiredMappingHandlerPatch {
    [HarmonyPrefix]
    [HarmonyPatch("GetCurrentControllerActionElementId", typeof(int), typeof(ControllerType), typeof(int))]
    public static bool GetCurrentControllerActionElementId(
        ref int __result,
        int actionId,
        ControllerType controllerType,
        int playerId = 0
    ) {
        if (!Plugin.CyberheadConfig.General.VrEnabled.Value) return true;
        // This crashes with a custom controller so let's just set it to a random glyph and worry about it later
        __result = 3;
        return false;
    }
}

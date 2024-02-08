using HarmonyLib;
using Reptile;

namespace Cyberhead.Patches;

[HarmonyPatch(typeof(GameplayCamera))]
public class GameplayCameraPatch {
    [HarmonyPrefix]
    [HarmonyPatch("UpdateCamera")]
    public static bool UpdateCamera(GameplayCamera __instance) {
        if (!Plugin.CyberheadConfig.General.VrEnabled.Value) return true;
        return false;
    }
}

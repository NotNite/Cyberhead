using HarmonyLib;
using Reptile;
using UnityEngine;

namespace Cyberhead.Patches;

[HarmonyPatch(typeof(PlayerPhoneCameras))]
public class PlayerPhoneCamerasPatch {
    [HarmonyPrefix]
    [HarmonyPatch("Awake")]
    [HarmonyBefore("SlopCrew.Plugin.Harmony")]
    public static bool Awake(PlayerPhoneCameras __instance) {
        if (!Plugin.CyberheadConfig.General.VrEnabled.Value) return true;

        // Mirror dwelling disabled due to technical difficulties
        var rearCameraObj = __instance.transform.Find("rearCamera");
        if (rearCameraObj != null) {
            var rearCamera = rearCameraObj.GetComponent<Camera>();
            if (rearCamera != null) rearCamera.enabled = false;
        }

        var frontCameraObj = __instance.transform.Find("frontCamera");
        if (frontCameraObj != null) {
            var frontCamera = frontCameraObj.GetComponent<Camera>();
            if (frontCamera != null) frontCamera.enabled = false;
        }

        return false;
    }
}

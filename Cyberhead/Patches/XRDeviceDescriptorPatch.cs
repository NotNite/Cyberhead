using System.Text.Json;
using HarmonyLib;
using UnityEngine.InputSystem.XR;

namespace Cyberhead.Patches;

[HarmonyPatch(typeof(XRDeviceDescriptor))]
public class XRDeviceDescriptorPatch {
    [HarmonyPostfix]
    [HarmonyPatch("FromJson")]
    public static void FromJson(ref XRDeviceDescriptor __result, string json) {
        try {
            // Try and set inputFeatures because unity is giving up doing it itself I guess
            var obj = JsonSerializer.Deserialize<XRDeviceDescriptor>(json, new JsonSerializerOptions {
                IncludeFields = true,
                PropertyNameCaseInsensitive = true
            });
            if (obj != null) __result.inputFeatures = obj.inputFeatures;
        } catch {
            // ignored
        }
    }
}

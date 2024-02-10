using HarmonyLib;
using Reptile;

namespace Cyberhead.Patches;

[HarmonyPatch(typeof(GameInput))]
public class GameInputPatch {
    [HarmonyPostfix]
    [HarmonyPatch("IsPlayerControllerJoystick")]
    public static void IsPlayerControllerJoystick(ref bool __result, int playerId = 0) {
        if (!Plugin.CyberheadConfig.General.VrEnabled.Value) return;
        __result = true;
    }
}

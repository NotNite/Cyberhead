using HarmonyLib;
using Reptile;
using UnityEngine;

namespace Cyberhead.Patches;

[HarmonyPatch(typeof(Player))]
public class PlayerPatch {
    [HarmonyPatch("Init")]
    [HarmonyPostfix]
    public static void Init(
        Player __instance,
        CharacterConstructor characterConstructor,
        Characters setCharacter = Characters.NONE,
        int setOutfit = 0,
        PlayerType setPlayerType = PlayerType.HUMAN,
        MoveStyle setMoveStyleEquipped = MoveStyle.ON_FOOT,
        Crew setCrew = Crew.PLAYERS
    ) {
        if (!Plugin.CyberheadConfig.General.VrEnabled.Value) return;
        if (__instance.isAI) return;
        Utils.CreateRig(__instance.transform.position, __instance.transform.rotation, __instance);
        Utils.DeleteCharacterHead(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch("SetCharacter")]
    public static void SetCharacter(Player __instance, Characters setChar, int setOutfit = 0) {
        if (!Plugin.CyberheadConfig.General.VrEnabled.Value) return;
        Utils.ApplyIk(__instance.characterVisual);
        if (!__instance.isAI) {
            Utils.AlignHead(__instance.transform.position, __instance.headTf.position);
            Utils.DeleteCharacterHead(__instance);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch("SetPosAndRotHard")]
    public static void SetPosAndRotHard(Player __instance, Vector3 pos, Quaternion rot) {
        // We can't determine if a player is a Slop Crew player or not when this is called in player init
        if (__instance.characterConstructor == null) return;

        if (!__instance.isAI && Plugin.XRRig != null) {
            Plugin.XRRig.transform.Find("CameraOffset/XR Camera").GetComponent<XRCamera>().MoveWithPlayer(pos);
        }
    }
}

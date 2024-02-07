using HarmonyLib;
using Reptile;
using Unity.XR.CoreUtils;
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
        if (__instance.isAI) return;

        var origin = new GameObject("XROrigin");
        origin.transform.SetParent(__instance.transform, false);
        var originComponent = origin.AddComponent<XROrigin>();

        var cameraOffset = new GameObject("CameraOffset");
        cameraOffset.transform.SetParent(origin.transform, false);

        // Need to make a new camera because GameplayCamera is completely black
        // TODO: figure out how to fix GameplayCamera being black and just use it instead
        var newCamera = new GameObject("XR Camera");
        newCamera.transform.SetParent(cameraOffset.transform, false);
        var newCameraComponent = newCamera.AddComponent<Camera>();

        originComponent.Camera = newCameraComponent;
        originComponent.CameraFloorOffsetObject = cameraOffset;

        originComponent.CameraYOffset = __instance.characterVisual.head.position.y - __instance.transform.position.y;
        cameraOffset.transform.localPosition = new Vector3(0, -originComponent.CameraYOffset, 0);

        newCamera.AddComponent<XRCamera>();

        // Incredible jank to get the player body (but not head) to show up in VR
        __instance.headTf.localScale = Vector3.zero;

        // Make the hands
        var handL = new GameObject("XR Hand L");
        handL.transform.SetParent(cameraOffset.transform, false);
        var handR = new GameObject("XR Hand R");
        handR.transform.SetParent(cameraOffset.transform, false);

        // These don't seem right but idc
        var ikL = new GameObject("IK");
        ikL.transform.SetParent(handL.transform, false);
        ikL.transform.rotation = Quaternion.Euler(90, 0, 0);
        var ikR = new GameObject("IK");
        ikR.transform.SetParent(handR.transform, false);
        ikR.transform.rotation = Quaternion.Euler(-90, 0, 0);

        handL.AddComponent<XRHand>().Init(true);
        handR.AddComponent<XRHand>().Init(false);

        ApplyIK(__instance);
        __instance.characterVisual.handIKTargetL = handL.transform;
        __instance.characterVisual.handIKTargetR = handR.transform;
    }

    [HarmonyPostfix]
    [HarmonyPatch("FixedUpdatePlayer")]
    public static void FixedUpdatePlayer(Player __instance) {
        if (!__instance.isAI) {
            __instance.characterVisual.handIKActiveL = true;
            __instance.characterVisual.handIKActiveR = true;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch("SetCharacter")]
    public static void SetCharacter(Player __instance, Characters setChar, int setOutfit = 0) {
        if (!__instance.isAI && __instance.tf.Find("XROrigin") != null) {
            ApplyIK(__instance);
        }
    }

    private static void ApplyIK(Player player) {
        player.characterVisual.handIKActiveL = true;
        player.characterVisual.handIKActiveR = true;
        player.characterVisual.handIKTargetL = player.tf.Find("XROrigin/CameraOffset/XR Hand L/IK");
        player.characterVisual.handIKTargetR = player.tf.Find("XROrigin/CameraOffset/XR Hand R/IK");
    }
}

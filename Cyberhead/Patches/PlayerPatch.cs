﻿using HarmonyLib;
using Reptile;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

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

        var origin = new GameObject("XROrigin");
        Plugin.XRRig = origin;
        origin.transform.position = __instance.transform.position;
        origin.transform.rotation = __instance.transform.rotation;
        var originComponent = origin.AddComponent<XROrigin>();
        originComponent.m_OriginBaseGameObject = origin;

        var cameraOffset = new GameObject("CameraOffset");
        cameraOffset.transform.SetParent(origin.transform, false);
        originComponent.CameraFloorOffsetObject = cameraOffset;

        // Need to make a new camera because GameplayCamera is completely black
        // TODO: figure out how to fix GameplayCamera being black and just use it instead
        var newCamera = new GameObject("XR Camera");
        newCamera.transform.SetParent(cameraOffset.transform, false);

        originComponent.CameraYOffset = __instance.characterVisual.head.position.y - __instance.transform.position.y;
        cameraOffset.transform.localPosition = new Vector3(0, -originComponent.CameraYOffset, 0);

        newCamera.AddComponent<XRCamera>();
        Core.Instance.UIManager.transform.Find("UICamera").GetComponent<Camera>().enabled = false;

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
        ikL.transform.localRotation = Quaternion.Euler(0, 0, 90);
        var ikR = new GameObject("IK");
        ikR.transform.SetParent(handR.transform, false);
        ikR.transform.localRotation = Quaternion.Euler(0, 0, -90);

        handL.AddComponent<XRHand>().Init(true);
        handR.AddComponent<XRHand>().Init(false);

        ApplyIK(__instance);
        __instance.characterVisual.handIKTargetL = ikL.transform;
        __instance.characterVisual.handIKTargetR = ikR.transform;

        // Add snap turn
        var locomotionSystem = origin.AddComponent<LocomotionSystem>();
        locomotionSystem.xrOrigin = originComponent;

        var snapTurn = origin.AddComponent<ActionBasedSnapTurnProvider>();
        snapTurn.system = locomotionSystem;
        snapTurn.turnAmount = 45;
        snapTurn.debounceTime = 0.25f;
        snapTurn.enableTurnLeftRight = true;
        snapTurn.enableTurnAround = false;
        snapTurn.rightHandSnapTurnAction = new InputActionProperty(Inputs.RightStickTurn);

        var xrHud = new GameObject("XR HUD");
        xrHud.transform.SetParent(newCamera.transform, false);
        xrHud.transform.localPosition = new Vector3(0, 0, 2f);

        Core.Instance.UIManager.gameplay.transform.parent.gameObject.AddComponent<XRHud>();
    }

    [HarmonyPostfix]
    [HarmonyPatch("FixedUpdatePlayer")]
    public static void FixedUpdatePlayer(Player __instance) {
        __instance.characterVisual.handIKActiveL = __instance.characterVisual.handIKTargetL != null;
        __instance.characterVisual.handIKActiveR = __instance.characterVisual.handIKTargetR != null;
    }

    [HarmonyPostfix]
    [HarmonyPatch("SetCharacter")]
    public static void SetCharacter(Player __instance, Characters setChar, int setOutfit = 0) {
        if (!Plugin.CyberheadConfig.General.VrEnabled.Value) return;
        if (!__instance.isAI) ApplyIK(__instance);
    }

    private static void ApplyIK(Player player) {
        player.characterVisual.handIKActiveL = true;
        player.characterVisual.handIKActiveR = true;
        if (Plugin.XRRig != null) {
            player.characterVisual.handIKTargetL = Plugin.XRRig.transform.Find("CameraOffset/XR Hand L/IK");
            player.characterVisual.handIKTargetR = Plugin.XRRig.transform.Find("CameraOffset/XR Hand R/IK");
        }
    }
}

using Reptile;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Cyberhead;

public class Utils {
    public const float HeadHeight = 1.6f;

    public static void CreateRig(
        Vector3 position, Quaternion rotation, Player? player
    ) {
        if (Plugin.XRRig != null) Object.Destroy(Plugin.XRRig);

        Plugin.XRRig = new GameObject("XROrigin");
        Plugin.XRRig.transform.position = position;
        Plugin.XRRig.transform.rotation = rotation;

        var origin = Plugin.XRRig.AddComponent<XROrigin>();
        origin.m_OriginBaseGameObject = Plugin.XRRig;

        var cameraOffset = new GameObject("CameraOffset");
        cameraOffset.transform.SetParent(Plugin.XRRig.transform, false);
        origin.CameraFloorOffsetObject = cameraOffset;
        AlignHead(position, player != null ? player.headTf.position : position);
        // This should really be done in a custom component
        if (player != null) player.headTf.localScale = Vector3.zero;

        // We'll move the GameplayCamera to this later
        var newCamera = new GameObject("XR Camera");
        newCamera.transform.SetParent(cameraOffset.transform, false);
        newCamera.AddComponent<XRCamera>();
        Core.Instance.UIManager.transform.Find("UICamera").GetComponent<Camera>().enabled = false;

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

        if (player != null) {
            player.characterVisual.handIKActiveL = true;
            player.characterVisual.handIKActiveR = true;
            player.characterVisual.handIKTargetL = ikL.transform;
            player.characterVisual.handIKTargetR = ikR.transform;
        }

        // Add snap turn
        var locomotionSystem = Plugin.XRRig.AddComponent<LocomotionSystem>();
        locomotionSystem.xrOrigin = origin;

        var snapTurn = Plugin.XRRig.AddComponent<ActionBasedSnapTurnProvider>();
        snapTurn.system = locomotionSystem;
        snapTurn.turnAmount = 45;
        snapTurn.debounceTime = 0.25f;
        snapTurn.enableTurnLeftRight = true;
        snapTurn.enableTurnAround = false;
        snapTurn.rightHandSnapTurnAction = new InputActionProperty(Inputs.RightStickTurn);

        // We'll move the HUD to this later
        var xrHud = new GameObject("XR HUD");
        xrHud.transform.SetParent(newCamera.transform, false);
        xrHud.transform.localPosition = new Vector3(0, 0, 2f);

        // Move the rig to the top of the scene so our camera renders first
        Plugin.XRRig.transform.SetAsFirstSibling();

        // TODO
        Core.Instance.UIManager.gameplay.transform.parent.gameObject.AddComponent<XRHud>();
    }

    public static void AlignHead(Vector3 position, Vector3 headPosition) {
        if (Plugin.XRRig == null) return;
        var origin = Plugin.XRRig!.GetComponent<XROrigin>();
        origin.CameraYOffset = headPosition.y - position.y;
        origin.CameraFloorOffsetObject.transform.localPosition = new Vector3(0, -origin.CameraYOffset, 0);
    }

    public static void ApplyIk(CharacterVisual characterVisual) {
        var origPlayer = characterVisual.gameObject.transform.parent;
        if (origPlayer == null) return;
        origPlayer = origPlayer.parent;
        if (origPlayer == null) return;

        if (Plugin.XRRig != null && !origPlayer.GetComponent<Player>().isAI) {
            characterVisual.handIKTargetL = Plugin.XRRig.transform.Find("CameraOffset/XR Hand L/IK");
            characterVisual.handIKTargetR = Plugin.XRRig.transform.Find("CameraOffset/XR Hand R/IK");
            characterVisual.handIKActiveL = true;
            characterVisual.handIKActiveR = true;
        } else if (origPlayer.Find("IKL") != null) {
            characterVisual.handIKTargetL = origPlayer.Find("IKL");
            characterVisual.handIKTargetR = origPlayer.Find("IKR");
            characterVisual.handIKActiveL = true;
            characterVisual.handIKActiveR = true;
        }
    }
}

using Reptile;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace Cyberhead;

public class Utils {
    public static void AlignHead(Player player) {
        if (Plugin.XRRig == null) return;
        player.headTf.localScale = Vector3.zero;
        var origin = Plugin.XRRig!.GetComponent<XROrigin>();
        origin.CameraYOffset = player.headTf.position.y - player.transform.position.y;
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
        } else if (origPlayer.Find("IKL") != null) {
            characterVisual.handIKTargetL = origPlayer.Find("IKL");
            characterVisual.handIKTargetR = origPlayer.Find("IKR");
        }
        characterVisual.handIKActiveL = characterVisual.handIKTargetL != null;
        characterVisual.handIKActiveR = characterVisual.handIKTargetR != null;
    }
}

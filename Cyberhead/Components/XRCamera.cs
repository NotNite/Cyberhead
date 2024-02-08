using Reptile;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Cyberhead;

public class XRCamera : MonoBehaviour {
    private TrackedPoseDriver trackedPoseDriver = null!;
    private XROrigin origin = null!;

    private void Awake() {
        this.trackedPoseDriver = this.gameObject.AddComponent<TrackedPoseDriver>();
        this.trackedPoseDriver.rotationAction = Inputs.HMDLook;
        this.trackedPoseDriver.positionAction = Inputs.HMDMove;
        this.trackedPoseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;

        var cameraOffset = this.gameObject.transform.parent;
        this.origin = cameraOffset.parent.gameObject.GetComponent<XROrigin>();
    }

    private void Update() {
        var worldHandler = WorldHandler.instance;
        if (worldHandler == null) return;
        var player = worldHandler.GetCurrentPlayer();
        if (player != null) {
            var tf = this.transform;
            var position = tf.position;
            var rotation = tf.rotation;

            if (player.GetVelocity().magnitude > 0) {
                // Player is moving, we need to follow along
                var anchorOffset = this.transform.position - this.origin.transform.position;
                anchorOffset.y = 0;
                var playerPos = player.transform.position;
                this.origin.transform.position = playerPos - anchorOffset;
            } else {
                // Snap the player to us when walking around in playspace
                const float posThreshold = 0f;
                const float rotThreshold = 0f;

                var moveToPos = position with {y = player.transform.position.y};
                var posDiff = player.transform.position - moveToPos;
                if (posDiff.magnitude >= posThreshold) player.motor.RigidbodyMove(moveToPos);

                var moveToRot = rotation;
                moveToRot = Quaternion.Euler(0, moveToRot.eulerAngles.y, 0);
                var rotDiff = player.transform.rotation.eulerAngles.y - moveToRot.eulerAngles.y;
                if (rotDiff >= rotThreshold) player.motor.RigidbodyMoveRotation(moveToRot);
            }

            // Place the camera onto us so rotation works properly
            var cam = player.cam;
            if (cam != null) {
                this.origin.Camera = cam.cam;
                cam.cam.nearClipPlane = 0.01f;

                var camTf = cam.transform;
                camTf.position = position;
                camTf.rotation = rotation;
            }
        }
    }
}

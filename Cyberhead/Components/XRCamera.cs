using Reptile;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Cyberhead;

public class XRCamera : MonoBehaviour {
    private TrackedPoseDriver trackedPoseDriver = null!;
    private GameObject origin = null!;

    private void Awake() {
        this.trackedPoseDriver = this.gameObject.AddComponent<TrackedPoseDriver>();
        this.trackedPoseDriver.rotationAction = Inputs.HMDLook;
        this.trackedPoseDriver.positionAction = Inputs.HMDMove;
        this.trackedPoseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;

        var cameraOffset = this.gameObject.transform.parent;
        this.origin = cameraOffset.parent.gameObject;
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
                var moveToPos = position with {y = player.transform.position.y};
                player.motor.RigidbodyMove(moveToPos);
                player.motor.RigidbodyMoveRotation(rotation);
            }

            // Place the camera onto us so rotation works properly
            var cam = player.cam;
            if (cam != null) {
                var camTf = cam.transform;
                camTf.position = position;
                camTf.rotation = rotation;
            }
        }
    }
}

using Reptile;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Cyberhead;

public class XRCamera : MonoBehaviour {
    private TrackedPoseDriver trackedPoseDriver = null!;

    private void Awake() {
        this.trackedPoseDriver = this.gameObject.AddComponent<TrackedPoseDriver>();
        this.trackedPoseDriver.rotationAction = Inputs.HMDLook;
        this.trackedPoseDriver.positionAction = Inputs.HMDMove;
        this.trackedPoseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
    }

    private void Update() {
        var worldHandler = WorldHandler.instance;
        if (worldHandler == null) return;
        var player = worldHandler.GetCurrentPlayer();
        if (player != null) {
            var tf = this.transform;
            var position = tf.position;
            var rotation = tf.rotation;

            // TODO: align the player object with the camera so you don't have to stationary in playspace

            var cam = player.cam;
            if (cam != null) {
                var camTf = cam.transform;
                camTf.position = position;
                camTf.rotation = rotation;
            }
        }
    }
}

using Reptile;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Cyberhead;

public class XRCamera : MonoBehaviour {
    public Vector3? LockedPos;

    private TrackedPoseDriver trackedPoseDriver = null!;
    private XROrigin origin = null!;
    private Camera theaterCamera = null!;

    private void Awake() {
        this.trackedPoseDriver = this.gameObject.AddComponent<TrackedPoseDriver>();
        this.trackedPoseDriver.rotationAction = Inputs.HMDLook;
        this.trackedPoseDriver.positionAction = Inputs.HMDMove;
        this.trackedPoseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;

        var cameraOffset = this.gameObject.transform.parent;
        this.origin = cameraOffset.parent.gameObject.GetComponent<XROrigin>();
        this.theaterCamera = this.gameObject.AddComponent<Camera>();
        this.theaterCamera.enabled = false;
        this.theaterCamera.depth = 0;
    }

    private void LateUpdate() {
        var player = this.GetCurrentPlayer();

        if (this.LockedPos != null) {
            this.origin.transform.position = this.LockedPos.Value;
            this.origin.transform.rotation = Quaternion.identity;
            this.theaterCamera.enabled = true;

            foreach (var cam in Camera.allCameras) {
                if (cam == this.theaterCamera) continue;
                if (cam.GetComponent<GameplayCamera>() != null) continue;
                cam.targetTexture = Plugin.CutsceneRenderTexture;
                break;
            }

            return;
        } else {
            if (this.theaterCamera.enabled) {
                this.theaterCamera.enabled = false;
                foreach (var cam in Camera.allCameras) {
                    if (cam.targetTexture == Plugin.CutsceneRenderTexture) cam.targetTexture = null;
                }
                if (player != null) this.MoveWithPlayer(player.transform.position);
            }
        }

        if (player != null) {
            var tf = this.transform;
            var position = tf.position;
            var rotation = tf.rotation;

            // Check for userInputEnabled so the game can teleport us when we die
            if (player.GetVelocity().magnitude > 0 || !player.userInputEnabled) {
                this.MoveWithPlayer(player.transform.position);
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

    private Player? GetCurrentPlayer() {
        var worldHandler = WorldHandler.instance;
        if (worldHandler == null) return null;
        var player = worldHandler.GetCurrentPlayer();
        return player;
    }

    public void MoveWithPlayer(Vector3 playerPos) {
        var anchorOffset = this.transform.position - this.origin.transform.position;
        anchorOffset.y = 0;
        this.origin.transform.position = playerPos - anchorOffset;
    }
}

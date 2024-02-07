using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Cyberhead;

public class XRHand : MonoBehaviour {
    public void Init(bool isLeft) {
        var trackedPoseDriver = this.gameObject.AddComponent<TrackedPoseDriver>();
        trackedPoseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
        trackedPoseDriver.positionAction = isLeft ? Inputs.LeftControllerMove : Inputs.RightControllerMove;
        trackedPoseDriver.rotationAction = isLeft ? Inputs.LeftControllerRotate : Inputs.RightControllerRotate;
    }
}

using UnityEngine.InputSystem;

namespace Cyberhead;

public class Inputs {
    public static InputAction HMDLook = null!;
    public static InputAction HMDMove = null!;
    public static InputAction LeftControllerRotate = null!;
    public static InputAction LeftControllerMove = null!;
    public static InputAction RightControllerRotate = null!;
    public static InputAction RightControllerMove = null!;

    // TODO: bindings and a Rewired forwarder

    public static void Init() {
        HMDLook = new InputAction("HMDLook", InputActionType.Value,
                                  "<XRHMD>/centerEyeRotation",
                                  expectedControlType: "Quaternion");
        HMDMove = new InputAction("HMDMove", InputActionType.Value,
                                  "<XRHMD>/centerEyePosition",
                                  expectedControlType: "Vector3");

        LeftControllerRotate = new InputAction("LeftControllerRotate", InputActionType.Value,
                                               "<XRController>{LeftHand}/deviceRotation",
                                               expectedControlType: "Quaternion");
        LeftControllerMove = new InputAction("LeftControllerMove", InputActionType.Value,
                                             "<XRController>{LeftHand}/devicePosition",
                                             expectedControlType: "Vector3");

        RightControllerRotate = new InputAction("RightControllerRotate", InputActionType.Value,
                                                "<XRController>{RightHand}/deviceRotation",
                                                expectedControlType: "Quaternion");
        RightControllerMove = new InputAction("RightControllerMove", InputActionType.Value,
                                              "<XRController>{RightHand}/devicePosition",
                                              expectedControlType: "Vector3");

        HMDLook.Enable();
        HMDMove.Enable();
        LeftControllerRotate.Enable();
        LeftControllerMove.Enable();
        RightControllerRotate.Enable();
        RightControllerMove.Enable();
    }
}

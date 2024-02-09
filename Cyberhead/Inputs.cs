using UnityEngine.InputSystem;

namespace Cyberhead;

public class Inputs {
    public static InputAction HMDLook = null!;
    public static InputAction HMDMove = null!;
    public static InputAction LeftControllerRotate = null!;
    public static InputAction LeftControllerMove = null!;
    public static InputAction RightControllerRotate = null!;
    public static InputAction RightControllerMove = null!;
    public static InputAction LeftStickMove = null!;
    public static InputAction RightContollerJump = null!;
    public static InputAction RightControllerSwitchStyle = null!;
    public static InputAction RightStickTurn = null!;
    public static InputAction RightTriggerManual = null!;
    public static InputAction RightGripBoost = null!;
    public static InputAction LeftControllerTrickOne = null!;
    public static InputAction LeftControllerTrickTwo = null!;
    public static InputAction LeftControllerTrickThree = null!;

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

        LeftStickMove = new InputAction("LeftStickMove", InputActionType.Value,
                                        "<XRController>{LeftHand}/primary2DAxis",
                                        expectedControlType: "Vector2");

        RightContollerJump = new InputAction("RightContollerJump", InputActionType.Button,
                                             "<XRController>{RightHand}/primaryButton");
        RightControllerSwitchStyle = new InputAction("RightControllerSwitchStyle", InputActionType.Button,
                                                     "<XRController>{RightHand}/secondaryButton");
        RightStickTurn = new InputAction("RightStickTurn", InputActionType.Value,
                                         "<XRController>{RightHand}/primary2DAxis",
                                         expectedControlType: "Vector2");
        RightTriggerManual = new InputAction("RightTriggerManual", InputActionType.Button,
                                             "<XRController>{RightHand}/triggerButton");
        RightGripBoost = new InputAction("RightGripBoost", InputActionType.Button,
                                         "<XRController>{RightHand}/gripButton");

        LeftControllerTrickOne = new InputAction("LeftControllerTrickOne", InputActionType.Button,
                                                 "<XRController>{LeftHand}/primaryButton");
        LeftControllerTrickTwo = new InputAction("LeftControllerTrickTwo", InputActionType.Button,
                                                 "<XRController>{LeftHand}/secondaryButton");
        LeftControllerTrickThree = new InputAction("LeftControllerTrickThree", InputActionType.Button,
                                                   "<XRController>{LeftHand}/{Primary2DAxisClick}");

        HMDLook.Enable();
        HMDMove.Enable();
        LeftControllerRotate.Enable();
        LeftControllerMove.Enable();
        RightControllerRotate.Enable();
        RightControllerMove.Enable();

        LeftStickMove.Enable();

        RightContollerJump.Enable();
        RightControllerSwitchStyle.Enable();
        RightStickTurn.Enable();
        RightTriggerManual.Enable();
        RightGripBoost.Enable();

        LeftControllerTrickOne.Enable();
        LeftControllerTrickTwo.Enable();
        LeftControllerTrickThree.Enable();
    }
}

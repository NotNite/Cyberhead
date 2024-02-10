using System.Collections.Generic;
using System.Linq;
using Reptile;
using Rewired;
using Rewired.Data.Mapping;
using UnityEngine;
using InputAction = UnityEngine.InputSystem.InputAction;
using InputActionType = UnityEngine.InputSystem.InputActionType;

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

    private static List<string> BufferedInputs = new();
    private static CustomController? Controller;
    private static CustomControllerMap? GameplayMap;
    private static CustomControllerMap? UiMap;

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

        Core.OnCoreInitialized += InitializeRewired;
    }

    private static void InitializeRewired() {
        // Create Rewired controller to forward
        ControllerElementIdentifier[] buttons = [
            new ControllerElementIdentifier(ButtonIDs.MoveX,
                                            "MoveX", "MoveX +", "MoveX -",
                                            ControllerElementType.Axis, false),
            new ControllerElementIdentifier(ButtonIDs.MoveY,
                                            "MoveY", "MoveY +", "MoveY -",
                                            ControllerElementType.Axis, false),
            new ControllerElementIdentifier(ButtonIDs.Jump,
                                            "Jump", "", "",
                                            ControllerElementType.Button, false),
            new ControllerElementIdentifier(ButtonIDs.SwitchStyle,
                                            "SwitchStyle", "", "",
                                            ControllerElementType.Button, false),
            new ControllerElementIdentifier(ButtonIDs.Manual,
                                            "Manual", "", "",
                                            ControllerElementType.Button, false),
            new ControllerElementIdentifier(ButtonIDs.Boost,
                                            "Boost", "", "",
                                            ControllerElementType.Button, false),
            new ControllerElementIdentifier(ButtonIDs.TrickOne,
                                            "TrickOne", "", "",
                                            ControllerElementType.Button, false),
            new ControllerElementIdentifier(ButtonIDs.TrickTwo,
                                            "TrickTwo", "", "",
                                            ControllerElementType.Button, false),
            new ControllerElementIdentifier(ButtonIDs.TrickThree,
                                            "TrickThree", "", "",
                                            ControllerElementType.Button, false)
        ];

        ReInput.UserData.AddCustomController();
        var controllerEditor = ReInput.UserData.customControllers.Last();
        foreach (var tmplButton in buttons) {
            if (tmplButton._elementType == ControllerElementType.Axis) {
                controllerEditor.AddAxis();
                controllerEditor.elementIdentifiers.RemoveAt(controllerEditor.elementIdentifiers.Count - 1);
                controllerEditor.elementIdentifiers.Add(tmplButton);
                var axis = controllerEditor.axes.Last();

                axis.name = tmplButton.name;
                axis.elementIdentifierId = tmplButton.id;
                axis.deadZone = 0.1f;
                axis.zero = 0;
                axis.min = -1;
                axis.max = 1;
                axis.invert = false;
                axis.axisInfo = new HardwareAxisInfo(AxisCoordinateMode.Absolute, false, 0f, SpecialAxisType.None);
                axis.range = AxisRange.Full;

                Plugin.Log.LogDebug($"Setting axis {axis.name} to {axis.elementIdentifierId}");
            } else {
                controllerEditor.AddButton();
                controllerEditor.elementIdentifiers.RemoveAt(controllerEditor.elementIdentifiers.Count - 1);
                controllerEditor.elementIdentifiers.Add(tmplButton);
                var button = controllerEditor.buttons.Last();

                button.name = tmplButton.name;
                button.elementIdentifierId = tmplButton.id;

                Plugin.Log.LogDebug($"Setting button {button.name} to {button.elementIdentifierId}");
            }
        }

        Controller = ReInput.controllers.CreateCustomController(controllerEditor.id);
        Controller.name = "Sega PlayStation";

        // Create the maps
        GameplayMap = BuildMap("Gameplay", 0, [
            new ActionElementMap(TRInputId.moveX, ControllerElementType.Axis, ButtonIDs.MoveX,
                                 Pole.Positive, AxisRange.Full),
            new ActionElementMap(TRInputId.moveY, ControllerElementType.Axis, ButtonIDs.MoveY,
                                 Pole.Positive, AxisRange.Full),
            new ActionElementMap(TRInputId.jump, ControllerElementType.Button, ButtonIDs.Jump),
            new ActionElementMap(TRInputId.switchStyle, ControllerElementType.Button, ButtonIDs.SwitchStyle),
            new ActionElementMap(TRInputId.slide, ControllerElementType.Button, ButtonIDs.Manual),
            new ActionElementMap(TRInputId.boost, ControllerElementType.Button, ButtonIDs.Boost),
            new ActionElementMap(TRInputId.trick1, ControllerElementType.Button, ButtonIDs.TrickOne),
            new ActionElementMap(TRInputId.trick2, ControllerElementType.Button, ButtonIDs.TrickTwo),
            new ActionElementMap(TRInputId.trick3, ControllerElementType.Button, ButtonIDs.TrickThree)
        ]);
        UiMap = BuildMap("UI", 1, [
            new ActionElementMap(TRInputId.menuX, ControllerElementType.Axis, ButtonIDs.MoveX,
                                 Pole.Positive, AxisRange.Full),
            new ActionElementMap(TRInputId.menuY, ControllerElementType.Axis, ButtonIDs.MoveY,
                                 Pole.Positive, AxisRange.Full),
            new ActionElementMap(TRInputId.menuConfirm, ControllerElementType.Button, ButtonIDs.Jump),
            new ActionElementMap(TRInputId.menuCancel, ControllerElementType.Button, ButtonIDs.SwitchStyle),
        ]);

        ReInput.InputSourceUpdateEvent += UpdateRewiredInput;
    }

    private static void UpdateRewiredInput() {
        Controller!.SetButtonValueById(ButtonIDs.Jump, FetchBuffer(RightContollerJump));
        Controller.SetButtonValueById(ButtonIDs.SwitchStyle, FetchBuffer(RightControllerSwitchStyle));
        Controller.SetButtonValueById(ButtonIDs.Manual, FetchBuffer(RightTriggerManual));
        Controller.SetButtonValueById(ButtonIDs.Boost, FetchBuffer(RightGripBoost));
        Controller.SetButtonValueById(ButtonIDs.TrickOne, FetchBuffer(LeftControllerTrickOne));
        Controller.SetButtonValueById(ButtonIDs.TrickTwo, FetchBuffer(LeftControllerTrickTwo));
        Controller.SetButtonValueById(ButtonIDs.TrickThree, FetchBuffer(LeftControllerTrickThree));

        var move = LeftStickMove.ReadValue<Vector2>();
        Controller.SetAxisValueById(ButtonIDs.MoveX, move.x);
        Controller.SetAxisValueById(ButtonIDs.MoveY, move.y);
    }

    private static CustomControllerMap BuildMap(string name, int category, ActionElementMap[] maps) {
        ReInput.UserData.CreateCustomControllerMap(category, Controller!.id, 0);
        var mapEditor = ReInput.UserData.customControllerMaps.Last();
        mapEditor.name = name;

        foreach (var tmplMap in maps) {
            mapEditor.AddActionElementMap();
            var action = mapEditor.GetActionElementMap(mapEditor.actionElementMaps.Count - 1);
            action.actionId = tmplMap.actionId;
            action.elementType = tmplMap.elementType;
            action.elementIdentifierId = tmplMap.elementIdentifierId;
            action.axisContribution = tmplMap.axisContribution;
            if (action.elementType == ControllerElementType.Axis) action.axisRange = tmplMap.axisRange;
            action.invert = tmplMap.invert;
            Plugin.Log.LogDebug($"Setting action {action.actionId} to {action.elementIdentifierId}");
        }

        // Search for signature CustomControllerMap (int _param1, int _param2, int _param3)
        return ReInput.UserData.NZnDxVQLTylWuKontBsWrRhUJsyL(category, Controller.id, 0);
    }

    // Controller inputs seem unreliable so we'll buffer them
    public static void Update() {
        InputAction[] inputs = [
            RightContollerJump,
            RightControllerSwitchStyle,
            RightTriggerManual,
            RightGripBoost,
            LeftControllerTrickOne,
            LeftControllerTrickTwo,
            LeftControllerTrickThree
        ];
        foreach (var input in inputs) {
            if (input.triggered && !BufferedInputs.Contains(input.name)) {
                BufferedInputs.Add(input.name);
            }
        }

        // Force assign Rewired controller
        var core = Core.Instance;
        if (Controller != null && GameplayMap != null && UiMap != null && core != null) {
            var rewired = core.GameInput;
            var player = rewired.FirstRewiredPlayer;

            if (!player.controllers.ContainsController(Controller)) {
                player.controllers.AddController(Controller, true);
                Controller.enabled = true;
            }

            if (player.controllers.maps.GetAllMaps(ControllerType.Custom).ToList().Count < 2) {
                CustomControllerMap[] maps = [GameplayMap, UiMap];
                foreach (var map in maps) {
                    if (player.controllers.maps
                            .GetMap(ControllerType.Custom, Controller.id, map.categoryId, map.layoutId) == null) {
                        player.controllers.maps.AddMap(Controller, map);
                    }

                    if (!map.enabled) map.enabled = true;
                }

                /*if (player.controllers.maps.GetMap(ControllerType.Custom, Controller.id, GameplayMap.categoryId,
                                                   GameplayMap.layoutId) ==
                    null) {
                    player.controllers.maps.AddMap(Controller, GameplayMap);
                }
                if (!GameplayMap.enabled) GameplayMap.enabled = true;
                */
            }
        }
    }

    public static bool FetchBuffer(InputAction action) {
        if (BufferedInputs.Contains(action.name)) {
            BufferedInputs.Remove(action.name);
            return true;
        }

        return action.triggered;
    }

    public class ButtonIDs {
        public const int MoveX = 0;
        public const int MoveY = 1;
        public const int Jump = 2;
        public const int SwitchStyle = 3;
        public const int Manual = 4;
        public const int Boost = 5;
        public const int TrickOne = 6;
        public const int TrickTwo = 7;
        public const int TrickThree = 8;
    }
}

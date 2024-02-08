﻿using HarmonyLib;
using Reptile;
using UnityEngine;

namespace Cyberhead.Patches;

[HarmonyPatch(typeof(UserInputHandler))]
public class UserInputHandlerPatch {
    [HarmonyPostfix]
    [HarmonyPatch("PollInputs")]
    public static void PollInputs(ref UserInputHandler.InputBuffer inputBuffer) {
        var move = Inputs.LeftStickMove.ReadValue<Vector2>();
        inputBuffer.moveAxisX = move.x;
        inputBuffer.moveAxisY = move.y;
        inputBuffer.jumpButtonNew = Inputs.RightContollerJump.triggered;
        inputBuffer.jumpButtonHeld = Inputs.RightContollerJump.ReadValue<float>() > 0.5f;
        inputBuffer.switchStyleButtonNew = Inputs.RightControllerSwitchStyle.triggered;
        inputBuffer.switchStyleButtonHeld = Inputs.RightControllerSwitchStyle.ReadValue<float>() > 0.5f;
        inputBuffer.boostButtonNew = Inputs.RightTriggerBoost.triggered;
        inputBuffer.boostButtonHeld = Inputs.RightTriggerBoost.ReadValue<float>() > 0.5f;
    }
}

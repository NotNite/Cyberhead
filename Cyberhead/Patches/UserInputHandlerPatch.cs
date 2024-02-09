using HarmonyLib;
using Reptile;
using UnityEngine;

namespace Cyberhead.Patches;

[HarmonyPatch(typeof(UserInputHandler))]
public class UserInputHandlerPatch {
    [HarmonyPostfix]
    [HarmonyPatch("PollInputs")]
    public static void PollInputs(ref UserInputHandler.InputBuffer inputBuffer) {
        if (!Plugin.CyberheadConfig.General.VrEnabled.Value) return;

        var move = Inputs.LeftStickMove.ReadValue<Vector2>();
        inputBuffer.moveAxisX = move.x;
        inputBuffer.moveAxisY = move.y;
        inputBuffer.jumpButtonNew = Inputs.RightContollerJump.triggered;
        inputBuffer.jumpButtonHeld = Inputs.RightContollerJump.ReadValue<float>() > 0.5f;
        inputBuffer.switchStyleButtonNew = Inputs.RightControllerSwitchStyle.triggered;
        inputBuffer.switchStyleButtonHeld = Inputs.RightControllerSwitchStyle.ReadValue<float>() > 0.5f;
        inputBuffer.slideButtonNew = Inputs.RightTriggerManual.triggered;
        inputBuffer.slideButtonHeld = Inputs.RightTriggerManual.ReadValue<float>() > 0.5f;
        inputBuffer.boostButtonNew = Inputs.RightGripBoost.triggered;
        inputBuffer.boostButtonHeld = Inputs.RightGripBoost.ReadValue<float>() > 0.5f;
        inputBuffer.trick1ButtonNew = Inputs.LeftControllerTrickOne.triggered;
        inputBuffer.trick1ButtonHeld = Inputs.LeftControllerTrickOne.ReadValue<float>() > 0.5f;
        inputBuffer.trick2ButtonNew = Inputs.LeftControllerTrickTwo.triggered;
        inputBuffer.trick2ButtonHeld = Inputs.LeftControllerTrickTwo.ReadValue<float>() > 0.5f;
        inputBuffer.trick3ButtonNew = Inputs.LeftControllerTrickThree.triggered;
        inputBuffer.trick3ButtonHeld = Inputs.LeftControllerTrickThree.ReadValue<float>() > 0.5f;
    }
}

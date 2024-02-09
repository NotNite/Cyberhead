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

        inputBuffer.jumpButtonNew = Inputs.FetchBuffer(Inputs.RightContollerJump);
        inputBuffer.jumpButtonHeld = inputBuffer.jumpButtonNew
                                     || Inputs.RightContollerJump.ReadValue<float>() > 0.5f;
        inputBuffer.switchStyleButtonNew = Inputs.FetchBuffer(Inputs.RightControllerSwitchStyle);
        inputBuffer.switchStyleButtonHeld = inputBuffer.switchStyleButtonNew
                                            || Inputs.RightControllerSwitchStyle.ReadValue<float>() > 0.5f;
        inputBuffer.slideButtonNew = Inputs.FetchBuffer(Inputs.RightTriggerManual);
        inputBuffer.slideButtonHeld = inputBuffer.slideButtonNew
                                      || Inputs.RightTriggerManual.ReadValue<float>() > 0.5f;
        inputBuffer.boostButtonNew = Inputs.FetchBuffer(Inputs.RightGripBoost);
        inputBuffer.boostButtonHeld = inputBuffer.boostButtonNew
                                      || Inputs.RightGripBoost.ReadValue<float>() > 0.5f;
        inputBuffer.trick1ButtonNew = Inputs.FetchBuffer(Inputs.LeftControllerTrickOne);
        inputBuffer.trick1ButtonHeld = inputBuffer.trick1ButtonNew
                                       || Inputs.LeftControllerTrickOne.ReadValue<float>() > 0.5f;
        inputBuffer.trick2ButtonNew = Inputs.FetchBuffer(Inputs.LeftControllerTrickTwo);
        inputBuffer.trick2ButtonHeld = inputBuffer.trick2ButtonNew
                                       || Inputs.LeftControllerTrickTwo.ReadValue<float>() > 0.5f;
        inputBuffer.trick3ButtonNew = Inputs.FetchBuffer(Inputs.LeftControllerTrickThree);
        inputBuffer.trick3ButtonHeld = inputBuffer.trick3ButtonNew
                                       || Inputs.LeftControllerTrickThree.ReadValue<float>() > 0.5f;
    }
}

using HarmonyLib;
using Reptile;
using UnityEngine;

namespace Cyberhead.Patches;

[HarmonyPatch(typeof(UserInputHandler))]
public class UserInputHandlerPatch {
    private static bool LastJumpButtonHeld;

    [HarmonyPostfix]
    [HarmonyPatch("PollInputs")]
    public static void PollInputs(ref UserInputHandler.InputBuffer inputBuffer) {
        var leftStickMove = Inputs.LeftStickMove.ReadValue<Vector2>();
        inputBuffer.moveAxisX = leftStickMove.x;
        inputBuffer.moveAxisY = leftStickMove.y;

        var jumpButtonPressed = Inputs.RightContollerJump.ReadValue<float>() > 0.5f;
        inputBuffer.jumpButtonNew = jumpButtonPressed && !LastJumpButtonHeld;
        inputBuffer.jumpButtonHeld = jumpButtonPressed;
        if (inputBuffer.jumpButtonNew) {
            Plugin.Log.LogInfo(
                $"jumpButtonNew={inputBuffer.jumpButtonNew} jumpButtonHeld={inputBuffer.jumpButtonHeld} LastJumpButtonHeld={LastJumpButtonHeld}");
        }
        LastJumpButtonHeld = jumpButtonPressed;
    }
}

using HarmonyLib;
using Reptile;

namespace Cyberhead.Patches;

[HarmonyPatch(typeof(CharacterConstructor))]
public class CharacterConstructorPatch {
    [HarmonyPrefix]
    [HarmonyPatch("CreateNewCharacterVisual")]
    public static void CreateNewCharacterVisual(
        CharacterConstructor __instance,
        ref bool IK
    ) {
        IK = true;
    }
}

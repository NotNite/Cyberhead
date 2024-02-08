using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Reptile;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.Interactions;

namespace Cyberhead;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInProcess("Bomb Rush Cyberfunk.exe")]
public class Plugin : BaseUnityPlugin {
    public static ManualLogSource Log = null!;
    public static Harmony Harmony = null!;
    public static Config CyberheadConfig = null!;
    public static GameObject? XRRig;
    public static SlopCrewSupport? SlopCrewSupport;

    private void Awake() {
        Log = this.Logger;
        Harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        CyberheadConfig = new Config(this.Config);
        SlopCrewSupport = new SlopCrewSupport();

        // Disabling VR means only Slop Crew support
        Harmony.PatchAll();
        if (!CyberheadConfig.General.VrEnabled.Value) return;

        InputSystem.PerformDefaultPluginInitialization();

        // Stolen from LCVR
        var valveIndex = ScriptableObject.CreateInstance<ValveIndexControllerProfile>();
        var hpReverb = ScriptableObject.CreateInstance<HPReverbG2ControllerProfile>();
        var htcVive = ScriptableObject.CreateInstance<HTCViveControllerProfile>();
        var mmController = ScriptableObject.CreateInstance<MicrosoftMotionControllerProfile>();
        var khrSimple = ScriptableObject.CreateInstance<KHRSimpleControllerProfile>();
        var metaQuestTouch = ScriptableObject.CreateInstance<MetaQuestTouchProControllerProfile>();
        var oculusTouch = ScriptableObject.CreateInstance<OculusTouchControllerProfile>();

        valveIndex.enabled = true;
        hpReverb.enabled = true;
        htcVive.enabled = true;
        mmController.enabled = true;
        khrSimple.enabled = true;
        metaQuestTouch.enabled = true;
        oculusTouch.enabled = true;

        OpenXRSettings.Instance.features = [
            valveIndex,
            hpReverb,
            htcVive,
            mmController,
            khrSimple,
            metaQuestTouch,
            oculusTouch
        ];

        // Completely stolen from LCVR please don't kill me
        // https://github.com/DaXcess/LCVR
        var generalSettings = ScriptableObject.CreateInstance<XRGeneralSettings>();
        var managerSettings = ScriptableObject.CreateInstance<XRManagerSettings>();
        var xrLoader = ScriptableObject.CreateInstance<OpenXRLoader>();

        generalSettings.Manager = managerSettings;
        ((List<XRLoader>) managerSettings.activeLoaders).Clear();
        ((List<XRLoader>) managerSettings.activeLoaders).Add(xrLoader);

        OpenXRSettings.Instance.renderMode = OpenXRSettings.RenderMode.MultiPass;
        OpenXRSettings.Instance.depthSubmissionMode = OpenXRSettings.DepthSubmissionMode.None;

        generalSettings.InitXRSDK();
        generalSettings.Start();

        Inputs.Init();
    }

    private void OnDestroy() {
        Harmony.UnpatchSelf();
        SlopCrewSupport?.Dispose();
    }
}

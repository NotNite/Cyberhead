using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;

namespace Cyberhead;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInProcess("Bomb Rush Cyberfunk.exe")]
public class Plugin : BaseUnityPlugin {
    public static ManualLogSource Log = null!;
    public static Harmony Harmony = null!;
    public static Config CyberheadConfig = null!;
    public static AssetBundle AssetBundle = null!;

    private void Awake() {
        Log = this.Logger;
        Harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        CyberheadConfig = new Config(this.Config);
        Harmony.PatchAll();

        InputSystem.PerformDefaultPluginInitialization();

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
    }
}

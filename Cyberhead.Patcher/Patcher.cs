using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using BepInEx;
using Mono.Cecil;

namespace Cyberhead.Patcher;

public static class Patcher {
    // Required for BepInEx to recognize the patcher
    public static IEnumerable<string> TargetDLLs { get; } = [];
    public static void Patch(AssemblyDefinition assembly) { }

    public static void Initialize() {
        var managed = Paths.ManagedPath;
        var pluginsDir = Path.Combine(managed, "..", "Plugins", "x86_64");
        var patcher = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var ggm = Path.Combine(managed, "..", "globalgamemanagers");
        var ggmBak = ggm + ".bak";
        if (!File.Exists(ggmBak)) File.Copy(ggm, ggmBak);

        // Patch globalgamemanagers (Project Settings)
        var assetsManager = new AssetsManager();
        assetsManager.LoadClassPackage(Path.Combine(patcher, "classdata.tpk"));
        var instance = assetsManager.LoadAssetsFile(ggmBak, false);
        var cldb = assetsManager.LoadClassDatabaseFromPackage("2021.3.20");

        var playerSettings = instance.file.GetAssetInfo(1);
        var playerSettingsBase = assetsManager.GetBaseField(instance, playerSettings);

        // Use both legacy and new input systems at once
        playerSettingsBase.Get("activeInputHandler").AsInt = 2;
        playerSettings.SetNewData(playerSettingsBase);

        using AssetsFileWriter writer = new(File.OpenWrite(ggm));
        instance.file.Write(writer);

        // Copy required native libraries
        string[] plugins = ["openxr_loader.dll", "UnityOpenXR.dll"];
        foreach (var plugin in plugins) {
            var from = Path.Combine(patcher, plugin);
            var to = Path.Combine(pluginsDir, plugin);
            if (!File.Exists(to)) File.Copy(from, to);
        }

        // Write some JSON files to make Unity happy I guess
        var subsystems = Path.Combine(Paths.ManagedPath, "..", "UnitySubsystems");
        WriteFile(
            Path.Combine(subsystems, "UnityOpenXR", "UnitySubsystemsManifest.json"),
            """{"name":"OpenXR XR Plugin","version":"1.8.2","libraryName":"UnityOpenXR","displays":[{"id":"OpenXR Display"}],"inputs":[{"id":"OpenXR Input"}]}"""
        );
        WriteFile(
            Path.Combine(subsystems, "OculusXRPlugin", "UnitySubsystemsManifest.json"),
            """{"name":"OculusXRPlugin","version":"1.0.0-preview","libraryName":"OculusXRPlugin","displays":[{"id":"oculus display","disablesLegacyVr":true,"supportedMirrorBlitReservedModes":["leftEye","rightEye","sideBySide","occlusionMesh"]}],"inputs":[{"id":"oculus input"}]}"""
        );
    }

    private static void WriteFile(string path, string content) {
        var dir = Path.GetDirectoryName(path)!;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(path, content);
    }
}

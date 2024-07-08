using BepInEx;
using BepInEx.Logging;
using CustomAssetImporter.Patches;
using CustomAssetImporter.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CustomAssetImporter
{
    [BepInPlugin("com.Arys.CustomAssetImporter", "Arys' Custom Asset Importer", "1.1.1")]
    public class ACAIPlugin : BaseUnityPlugin
    {
        internal const string EffectsPath = "assets\\effects\\";
        internal const string RigLayoutsPath = "assets\\rig_layouts\\";

        internal static string Directory;
        internal static ManualLogSource LogSource;

        private void Awake()
        {
            LogSource = Logger;
            Directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/";

            new AddCustomEffectsPatch().Enable();
            new GameWorldOnDestroyPatch().Enable();
        }

        private void Start()
        {
            InitCustomRigLayouts();
        }

        private void InitCustomRigLayouts()
        {
            string[] bundles = AssetLoader.GetBundlePathsFromDirectory(RigLayoutsPath);

            if (!bundles.Any())
            {
#if DEBUG
                LogSource.LogInfo($"\"{RigLayoutsPath}\" does not contain any bundles. No custom rig layouts will be added.");
#endif
                return;
            }

            Type type = typeof(CacheResourcesPopAbstractClass);
            var dictionary = (Dictionary<string, object>)AccessTools.DeclaredField(type, "dictionary_0").GetValue(null);

            FrameworkHelper.AddCustomRigLayouts(dictionary, bundles);
        }
    }
}

using Aki.Reflection.Patching;
using CustomAssetImporter.Templates;
using CustomAssetImporter.Util;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Systems.Effects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomAssetImporter.Patches
{
    internal class AddCustomEffectsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Effects), "InitDictionaryAndNames");
        }

        [PatchPrefix]
        private static void PatchPrefix(Effects __instance)
        {
            string directory = "assets\\effects\\";
            string[] effectsBundles = AssetLoader.GetBundlePathsFromDirectory(directory);
            if (!effectsBundles.Any())
            {
                Plugin.LogSource.LogWarning($"\"{directory}\" does not contain any bundles. No custom effects will be added.");
                return;
            }

            AddCustomEffects(__instance, effectsBundles);
        }

        private static void AddCustomEffects(Effects effects, string[] effectsBundles)
        {
            List<Effects.Effect> customEffectsList = [];
            foreach (string bundlePath in effectsBundles)
            {
                string bundleName = Path.GetFileName(bundlePath);
                string bundleRelativePath = AssetLoader.GetRelativePath(Plugin.Directory, bundlePath);
                GameObject gameObject = AssetLoader.LoadAsset(bundleRelativePath);
                GameObject instantiatedEffects = Object.Instantiate(gameObject);
                if (!instantiatedEffects.TryGetComponent<CustomEffectsTemplate>(out var customEffects))
                {
                    Plugin.LogSource.LogError($"\"{bundleName}\" does not contain a valid CustomEffects script. Skipping...");
                    continue;
                }

                if (!customEffects.EffectsArray.Any())
                {
                    Plugin.LogSource.LogError($"\"{bundleName}\" CustomEffects script has an empty EffectsArray. Skipping...");
                    continue;
                }

                if (customEffectsList.Any())
                {
                    bool hasDuplicates = FrameworkUtil.CheckForDuplicates(customEffectsList, customEffects.EffectsArray, new EffectComparer());
                    if (hasDuplicates)
                    {
                        Plugin.LogSource.LogError($"\"{bundleName}\" EffectsArray has an Effect with a conflicting Name property with an existing custom Effect Name. Skipping...");
                        continue;
                    }
                }

                customEffectsList.AddRange(customEffects.EffectsArray);
                Plugin.LogSource.LogInfo($"\"{bundleName}\"'s effects have been added.");

                foreach (var child in instantiatedEffects.transform.GetChildren())
                {
                    child.parent = effects.transform;
                }
            }

            effects.EffectsArray = effects.EffectsArray.AddRangeToArray([.. customEffectsList]);
        }
    }
}

using Aki.Reflection.Patching;
using CustomAssetImporter.Templates;
using CustomAssetImporter.Util;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Systems.Effects;
using UnityEngine;

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
            string directory = "assets/effects/";
            string[] effectsBundles = AssetLoader.ReadBundleNamesFromDirectory(directory);
            if (!effectsBundles.Any())
            {
                Plugin.LogSource.LogWarning($"\"{directory}\" does not contain any bundles. No custom effects will be added.");
                return;
            }

            AddCustomEffects(__instance, directory, effectsBundles);
        }

        private static void AddCustomEffects(Effects __instance, string directory, string[] effectsBundles)
        {
            List<Effects.Effect> customEffectsList = [];
            foreach (string bundle in effectsBundles)
            {
                GameObject gameObject = AssetLoader.LoadAsset($"{directory}{bundle}");
                if (!gameObject.TryGetComponent<CustomEffectsTemplate>(out var customEffects))
                {
                    Plugin.LogSource.LogError($"{bundle} does not contain a valid CustomEffects script. Skipping...");
                    continue;
                }

                if (!customEffects.EffectsArray.Any())
                {
                    Plugin.LogSource.LogError($"{bundle} CustomEffects script has an empty EffectsArray. Skipping...");
                    continue;
                }

                if (customEffectsList.Any())
                {
                    bool hasDuplicates = FrameworkUtil.CheckForDuplicates(customEffectsList, customEffects.EffectsArray, new EffectComparer());
                    if (hasDuplicates)
                    {
                        Plugin.LogSource.LogError($"{bundle} EffectsArray has an Effect with a conflicting Name property with an existing custom Effect Name. Skipping...");
                        continue;
                    }
                }

                customEffectsList.AddRange(customEffects.EffectsArray);
            }

            __instance.EffectsArray.AddRangeToArray([.. customEffectsList]);
        }
    }
}

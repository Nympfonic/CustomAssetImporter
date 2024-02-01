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
        private static async void PatchPrefix(Effects __instance)
        {
            string directory = "assets/systems/effects/particlesystems/";
            string[] effectsBundles = AssetLoader.ReadBundleNamesFromDirectory(directory);

            List<Effects.Effect> customEffectsList = [];
            foreach (string bundle in effectsBundles)
            {
                GameObject gameObject = await AssetLoader.LoadAssetAsync($"{directory}{bundle}");
                if (!gameObject.TryGetComponent<CustomEffectsTemplate>(out var customEffects))
                {
                    Plugin.LogSource.LogError($"{bundle} does not contain a valid CustomEffects component. Skipping...");
                    continue;
                }

                if (!customEffects.EffectsArray.Any())
                {
                    Plugin.LogSource.LogError($"{bundle} CustomEffects component has an empty EffectsArray. Skipping...");
                    continue;
                }

                if (customEffectsList.Any() && CheckForDuplicates(customEffectsList, customEffects.EffectsArray))
                {
                    Plugin.LogSource.LogError($"{bundle} EffectsArray has an Effect with a conflicting Name property with an existing custom Effect Name. Skipping...");
                    continue;
                }

                customEffectsList.AddRange(customEffects.EffectsArray);
            }

            Effects.Effect[] originalEffectsArray = __instance.EffectsArray;
            __instance.EffectsArray = originalEffectsArray.AddRangeToArray([.. customEffectsList]);
        }

        private static bool CheckForDuplicates(IEnumerable<Effects.Effect> source, IEnumerable<Effects.Effect> target)
        {
            var duplicates = source.Intersect(target, new EffectComparer());
            if (duplicates.Any())
            {
                return true;
            }

            return false;
        }
    }
}

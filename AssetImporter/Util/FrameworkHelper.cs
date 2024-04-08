using CustomAssetImporter.Templates;
using CustomAssetImporter.Util.Comparers;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Systems.Effects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomAssetImporter.Util
{
    internal static class FrameworkHelper
    {
        internal static void AddCustomEffects(Effects effects, string[] effectsBundles)
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

                // After first iteration, customEffectsList may be populated, so we check if there are duplicate Effects names
                if (customEffectsList.Any())
                {
                    bool hasDuplicates = CheckForDuplicates(customEffectsList, customEffects.EffectsArray, new EffectComparer());
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

        internal static void AddCustomRigLayouts()
        {

        }

        private static bool CheckForDuplicates<T>(IEnumerable<T> source, IEnumerable<T> target, IEqualityComparer<T> comparer)
        {
            return source.Intersect(target, comparer).Any();
        }
    }
}

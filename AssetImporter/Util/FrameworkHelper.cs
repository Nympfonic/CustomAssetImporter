using CustomAssetImporter.Templates;
using CustomAssetImporter.Util.Comparers;
using EFT.UI.DragAndDrop;
using HarmonyLib;
using System.Collections.Generic;
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
                string bundleRelativePath = AssetLoader.GetRelativePath(ACAIPlugin.Directory, bundlePath);

                GameObject gameObject = AssetLoader.LoadAsset(bundleRelativePath);
                GameObject instantiatedEffects = Object.Instantiate(gameObject);

                if (!instantiatedEffects.TryGetComponent<CustomEffectsTemplate>(out var customEffects))
                {
                    ACAIPlugin.LogSource.LogError($"\"{bundleRelativePath}\" prefab is missing a {nameof(CustomEffectsTemplate)} component. Skipping...");
                    continue;
                }

                if (!customEffects.EffectsArray.Any())
                {
                    ACAIPlugin.LogSource.LogError($"\"{bundleRelativePath}\" prefab\'s {nameof(CustomEffectsTemplate)} component has an empty {nameof(CustomEffectsTemplate.EffectsArray)}. Skipping...");
                    continue;
                }

                // After first iteration, customEffectsList may be populated, so we check if there are duplicate Effects names
                if (customEffectsList.Any())
                {
                    bool hasDuplicates = CheckForDuplicates(customEffectsList, customEffects.EffectsArray, new EffectComparer());
                    if (hasDuplicates)
                    {
                        ACAIPlugin.LogSource.LogError($"\"{bundleRelativePath}\" prefab\'s {nameof(CustomEffectsTemplate.EffectsArray)} has an Effect with a conflicting Name property with an existing custom Effect Name. Skipping...");
                        continue;
                    }
                }

                customEffectsList.AddRange(customEffects.EffectsArray);
#if DEBUG
                Plugin.LogSource.LogDebug($"\"{bundleRelativePath}\"\'s effects have been added.");
#endif

                foreach (var child in instantiatedEffects.transform.GetChildren())
                {
                    child.parent = effects.transform;
                }
            }

            effects.EffectsArray = effects.EffectsArray.AddRangeToArray([.. customEffectsList]);
        }

        internal static void AddCustomRigLayouts(IDictionary<string, object> rigLayoutDictionary, string[] rigLayoutBundles)
        {
            foreach (string bundlePath in rigLayoutBundles)
            {
                string bundleRelativePath = AssetLoader.GetRelativePath(ACAIPlugin.Directory, bundlePath);

                GameObject[] prefabs = AssetLoader.LoadAllAssets(bundleRelativePath);
                foreach (var prefab in prefabs)
                {
                    if (!prefab.TryGetComponent<ContainedGridsView>(out var gridView))
                    {
                        ACAIPlugin.LogSource.LogError($"\"{bundleRelativePath}\"\'s rig layout \"{prefab.name}\" is missing a {nameof(ContainedGridsView)} component.");
                        continue;
                    }

                    string key = $"UI/Rig Layouts/{prefab.name}";
                    if (rigLayoutDictionary.ContainsKey(key))
                    {
                        ACAIPlugin.LogSource.LogError($"\"{bundleRelativePath}\"\'s rig layout \"{prefab.name}\" conflicts with an existing key in the rig layout dictionary.");
                        continue;
                    }

                    rigLayoutDictionary.Add(key, gridView);
#if DEBUG
                    Plugin.LogSource.LogDebug($"\"{bundleRelativePath}\"\'s rig layout \"{prefab.name}\" has been added.");
#endif
                }

                AssetLoader.UnloadBundleByPath(bundleRelativePath, false, AssetLoader.LoadedBundles);
            }
        }

        private static bool CheckForDuplicates<T>(IEnumerable<T> source, IEnumerable<T> target, IEqualityComparer<T> comparer)
        {
            return source.Intersect(target, comparer).Any();
        }
    }
}

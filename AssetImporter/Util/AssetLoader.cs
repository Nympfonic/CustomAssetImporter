using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomAssetImporter.Util
{
    public static class AssetLoader
    {
        internal static readonly Dictionary<string, AssetBundle> LoadedBundles = [];
        internal static readonly Dictionary<string, AssetBundle> LoadedBundlesInRaid = [];

        /// <summary>
        /// This method is in newer releases of .NET but not .NET Framework 4.7.1
        /// </summary>
        /// <param name="relativeTo">The full path of the source directory</param>
        /// <param name="path">The full path of the destination directory</param>
        public static string GetRelativePath(string relativeTo, string path)
        {
            var uri = new Uri(relativeTo);
            var rel = Uri.UnescapeDataString(uri.MakeRelativeUri(new Uri(path)).ToString()).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (!rel.Contains(Path.DirectorySeparatorChar.ToString()))
            {
                rel = $".{Path.DirectorySeparatorChar}{rel}";
            }
            return rel;
        }

        /// <param name="path">Relative path from <see cref="ACAIPlugin.Directory"/></param>
        /// <returns>A string array containing relative paths for all bundles found within the specified path.</returns>
        public static string[] GetBundlePathsFromDirectory(string path)
        {
            return Directory.GetFiles(Path.Combine(ACAIPlugin.Directory, path), "*.bundle", SearchOption.AllDirectories);
        }

        /// <summary>
        /// Loads the first root-level GameObject from an asset bundle.
        /// </summary>
        /// <param name="bundle">Relative path to the bundle</param>
        /// <param name="assetName">Name of the GameObject to be loaded, including its child GameObjects</param>
        /// <param name="targetDictionary">The target dictionary to cache the loaded asset bundle in</param>
        /// <returns>A GameObject prefab which will need to be instantiated.</returns>
        public static GameObject LoadAsset(string bundle, string assetName = null, IDictionary<string, AssetBundle> targetDictionary = null)
        {
            return LoadAsset<GameObject>(bundle, assetName, targetDictionary);
        }

        public static T LoadAsset<T>(string bundle, string assetName = null, IDictionary<string, AssetBundle> targetDictionary = null)
            where T : Object
        {
            var assetBundle = LoadBundle(bundle, targetDictionary);

            var requestedBundle = string.IsNullOrEmpty(assetName)
                ? assetBundle.LoadAllAssets<T>()[0]
                : assetBundle.LoadAsset<T>(assetName);
            if (requestedBundle == null)
            {
                ACAIPlugin.LogSource.LogError($"Could not load object from bundle: {bundle}, asset list is empty");
                return null;
            }

            return requestedBundle;
        }

        /// <summary>
        /// Loads all root-level GameObjects from an asset bundle.
        /// </summary>
        /// <param name="bundle">Relative path to the bundle</param>
        /// <param name="targetDictionary">The target dictionary to cache the loaded asset bundle in</param>
        /// <returns>An array of GameObject prefabs from the root level of the bundle.</returns>
        public static GameObject[] LoadAllAssets(string bundle, IDictionary<string, AssetBundle> targetDictionary = null)
        {
            return LoadAllAssets<GameObject>(bundle, targetDictionary);
        }

        public static T[] LoadAllAssets<T>(string bundle, IDictionary<string, AssetBundle> targetDictionary = null)
            where T : Object
        {
            var assetBundle = LoadBundle(bundle, targetDictionary);

            var requestedBundle = assetBundle.LoadAllAssets<T>();
            if (requestedBundle.IsNullOrEmpty())
            {
                ACAIPlugin.LogSource.LogError($"Could not load objects from bundle: {bundle}, asset list is empty");
                return [];
            }

            return requestedBundle;
        }

        public static void UnloadBundleByPath(string bundlePath, bool unloadAllLoadedObjects, IDictionary<string, AssetBundle> targetDictionary)
        {
            if (targetDictionary.TryGetValue(bundlePath, out var bundle))
            {
                bundle.Unload(unloadAllLoadedObjects);
                targetDictionary.Remove(bundlePath);
            }
        }

        public static void UnloadBundle(AssetBundle bundle, bool unloadAllLoadedObjects, IDictionary<string, AssetBundle> targetDictionary)
        {
            if (targetDictionary.TryGetKey(bundle, out var bundlePath))
            {
                bundle.Unload(unloadAllLoadedObjects);
                targetDictionary.Remove(bundlePath);
            }
        }

        public static void UnloadAllBundles(bool unloadAllLoadedObjects, IDictionary<string, AssetBundle> targetDictionary)
        {
            foreach (var bundle in targetDictionary.Values)
            {
                bundle.Unload(unloadAllLoadedObjects);
            }

            targetDictionary.Clear();
        }

        private static AssetBundle LoadBundle(string bundleName, IDictionary<string, AssetBundle> targetDictionary = null)
        {
            string bundlePath = bundleName;
            targetDictionary ??= LoadedBundles;

            bundleName = Regex.Match(bundleName, @"[^//]*$").Value;
            if (targetDictionary.TryGetValue(bundleName, out var bundle))
            {
                return bundle;
            }

            var requestedBundle = AssetBundle.LoadFromFile(ACAIPlugin.Directory + bundlePath);
            if (requestedBundle != null)
            {
                targetDictionary.Add(bundleName, requestedBundle);
                return requestedBundle;
            }

            ACAIPlugin.LogSource.LogError($"Could not load bundle: {bundlePath}");
            return null;
        }
    }
}

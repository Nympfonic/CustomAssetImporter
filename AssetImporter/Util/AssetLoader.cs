using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomAssetImporter.Util
{
    internal static class AssetLoader
    {
        private static readonly Dictionary<string, AssetBundle> LoadedBundles = [];

        private static AssetBundle LoadBundle(string bundleName)
        {
            string bundlePath = bundleName;
            bundleName = Regex.Match(bundleName, @"[^//]*$").Value;
            if (LoadedBundles.TryGetValue(bundleName, out var bundle))
            {
                return bundle;
            }

            var requestedBundle = AssetBundle.LoadFromFile(Plugin.Directory + bundlePath);
            if (requestedBundle != null)
            {
                LoadedBundles.Add(bundleName, requestedBundle);
                return requestedBundle;
            }

            Plugin.LogSource.LogError($"Could not load bundle: {bundlePath}");
            return null;
        }

        private static async Task<AssetBundle> LoadBundleAsync(string bundleName)
        {
            string bundlePath = bundleName;
            bundleName = Regex.Match(bundleName, @"[^//]*$").Value;
            if (LoadedBundles.TryGetValue(bundleName, out var bundle))
            {
                return bundle;
            }

            var bundleRequest = AssetBundle.LoadFromFileAsync(Plugin.Directory + bundlePath);
            while (!bundleRequest.isDone)
            {
                await Task.Yield();
            }

            var requestedBundle = bundleRequest.assetBundle;
            if (requestedBundle != null)
            {
                LoadedBundles.Add(bundleName, requestedBundle);
                return requestedBundle;
            }

            Plugin.LogSource.LogError($"Could not load bundle: {bundlePath}");
            return null;
        }

        /// <summary>
        /// This method is in newer releases of .NET but not .NET Framework 4.7.1
        /// </summary>
        /// <param name="relativeTo">The full path of the source directory</param>
        /// <param name="path">The full path of the destination directory</param>
        internal static string GetRelativePath(string relativeTo, string path)
        {
            var uri = new Uri(relativeTo);
            var rel = Uri.UnescapeDataString(uri.MakeRelativeUri(new Uri(path)).ToString()).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (!rel.Contains(Path.DirectorySeparatorChar.ToString()))
            {
                rel = $".{Path.DirectorySeparatorChar}{rel}";
            }
            return rel;
        }

        /// <param name="path">Relative path from <see cref="Plugin.Directory"/></param>
        /// <returns>A string array containing relative paths for all bundles found within the specified path.</returns>
        internal static string[] GetBundlePathsFromDirectory(string path)
        {
            return Directory.GetFiles(Plugin.Directory + path, "*.bundle", SearchOption.AllDirectories);
        }

        /// <summary>
        /// Loads a GameObject from an asset bundle.
        /// </summary>
        /// <param name="bundle">Relative path to the bundle</param>
        /// <param name="assetName">Name of the GameObject to be loaded, including its child GameObjects</param>
        /// <returns>A GameObject which will need to be instantiated.</returns>
        internal static GameObject LoadAsset(string bundle, string assetName = null)
        {
            return LoadAsset<GameObject>(bundle, assetName);
        }

        internal static T LoadAsset<T>(string bundle, string assetName = null) where T : Object
        {
            var assetBundle = LoadBundle(bundle);

            var requestedBundle = string.IsNullOrEmpty(assetName)
                ? assetBundle.LoadAllAssets<T>()[0]
                : assetBundle.LoadAsset<T>(assetName);
            if (requestedBundle == null)
            {
                Plugin.LogSource.LogError($"Could not load object from bundle: {bundle}, asset list is empty");
                return null;
            }

            return requestedBundle;
        }

        /// <summary>
        /// Asynchronous version of <see cref="LoadAsset(string, string)"/>.
        /// </summary>
        internal static Task<GameObject> LoadAssetAsync(string bundle, string assetName = null)
        {
            return LoadAssetAsync<GameObject>(bundle, assetName);
        }

        internal static async Task<T> LoadAssetAsync<T>(string bundle, string assetName = null) where T : Object
        {
            var assetBundle = await LoadBundleAsync(bundle);

            var assetBundleRequest = string.IsNullOrEmpty(assetName)
                ? assetBundle.LoadAllAssetsAsync<T>()
                : assetBundle.LoadAssetAsync<T>(assetName);
            while (!assetBundleRequest.isDone)
            {
                await Task.Yield();
            }

            if (assetBundleRequest.allAssets.Length == 0)
            {
                Plugin.LogSource.LogError($"Could not load object from bundle: {bundle}, asset list is empty");
                return null;
            }

            return (T)assetBundleRequest.allAssets[0];
        }

        internal static void UnloadAllBundles()
        {
            foreach (var bundle in LoadedBundles.Values)
            {
                bundle.Unload(true);
            }

            LoadedBundles.Clear();
        }
    }
}

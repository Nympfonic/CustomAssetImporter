using Aki.Reflection.Patching;
using CustomAssetImporter.Util;
using EFT;
using HarmonyLib;
using System.Reflection;

namespace CustomAssetImporter.Patches
{
    /// <summary>
    /// Postfix patch on <see cref="GameWorld.OnDestroy"/> to unload all asset bundles that have an in-raid lifetime.
    /// </summary>
    internal class GameWorldOnDestroyPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.OnDestroy));
        }

        [PatchPostfix]
        private static void PatchPostfix()
        {
            AssetLoader.UnloadAllBundles(true, AssetLoader.LoadedBundlesInRaid);
        }
    }
}

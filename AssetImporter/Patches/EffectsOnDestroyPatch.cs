using Aki.Reflection.Patching;
using CustomAssetImporter.Util;
using EFT;
using HarmonyLib;
using System.Reflection;

namespace CustomAssetImporter.Patches
{
    internal class EffectsOnDestroyPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameWorld), "OnDestroy");
        }

        [PatchPostfix]
        private static void PatchPostfix()
        {
            AssetLoader.UnloadAllBundles();
        }
    }
}

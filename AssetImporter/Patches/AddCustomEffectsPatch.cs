using Aki.Reflection.Patching;
using CustomAssetImporter.Util;
using HarmonyLib;
using System.Linq;
using System.Reflection;
using Systems.Effects;

namespace CustomAssetImporter.Patches
{
    internal class AddCustomEffectsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Effects), nameof(Effects.InitDictionaryAndNames));
        }

        [PatchPrefix]
        private static void PatchPrefix(Effects __instance)
        {
            string directory = Plugin.EffectsDirectory;
            string[] effectsBundles = AssetLoader.GetBundlePathsFromDirectory(directory);

            if (!effectsBundles.Any())
            {
                Plugin.LogSource.LogWarning($"\"{directory}\" does not contain any bundles. No custom effects will be added.");
                return;
            }

            FrameworkHelper.AddCustomEffects(__instance, effectsBundles);
        }
    }
}

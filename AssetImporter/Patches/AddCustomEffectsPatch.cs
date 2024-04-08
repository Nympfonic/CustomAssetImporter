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
            string[] bundles = AssetLoader.GetBundlePathsFromDirectory(Plugin.EffectsDirectory);

            if (!bundles.Any())
            {
#if DEBUG
                Plugin.LogSource.LogInfo($"\"{Plugin.EffectsDirectory}\" does not contain any bundles. No custom effects will be added.");
#endif
                return;
            }

            FrameworkHelper.AddCustomEffects(__instance, bundles);
        }
    }
}

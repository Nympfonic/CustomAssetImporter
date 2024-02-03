using BepInEx;
using BepInEx.Logging;
using CustomAssetImporter.Patches;
using System.IO;
using System.Reflection;

namespace CustomAssetImporter
{
    [BepInPlugin("Arys-CustomAssetImporter", "Arys-CustomAssetImporter", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static string Directory;
        internal static ManualLogSource LogSource;

        private void Awake()
        {
            LogSource = Logger;
            Directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/";

            new AddCustomEffectsPatch().Enable();
            new EffectsOnDestroyPatch().Enable();
        }
    }
}

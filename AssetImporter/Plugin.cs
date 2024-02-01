using BepInEx;
using BepInEx.Logging;
using System.IO;
using System.Reflection;

namespace CustomAssetImporter
{
    [BepInPlugin("Arys-AssetImporter", "AssetImporter", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static string Directory;
        internal static ManualLogSource LogSource;

        private void Awake()
        {
            LogSource = Logger;
            Directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/";
        }
    }
}

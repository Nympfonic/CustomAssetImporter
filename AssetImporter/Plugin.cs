using BepInEx;
using BepInEx.Logging;
using System.IO;
using System.Reflection;

namespace CustomAssetImporter
{
    [BepInPlugin("com.Arys.CustomAssetImporter", "Arys' Custom Asset Importer", "1.0.1")]
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

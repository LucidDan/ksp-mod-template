using System.IO;
using JetBrains.Annotations;
using UnityEngine;

namespace SampleMod
{
    /// <summary>
    /// Mod Manager to provide any "global" management functionality;
    ///   - load prefabs, settings files, and any other resources that we might need.
    ///   - return mod name, vendor, version information.
    /// </summary>
    public class ModManager
    {
        private static string AssetBundleFilename => "samplemod.bundle";
        public static string Vendor => "LucidDan";
        public static string Name => "SampleMod";
        public static string Version => "1.0.0.0";
        public static string DisplayName => "Lucid Dan's Sample Mod";
        private static AssetBundle? _prefabs;

        [CanBeNull] private static AssetBundle Prefabs
        {
            get
            {
                if (_prefabs != null) return _prefabs;

                string assetPath = GetAbsoluteResourcePath(AssetBundleFilename);

                Utils.Log($"Loading asset bundle: {assetPath}");
                _prefabs = AssetBundle.LoadFromFile(assetPath);

                if (_prefabs != null) return _prefabs;

                Utils.Log("Failed to load asset bundle.");
                throw new IOException($"Could not load asset bundle '{assetPath}'.");
            }
        }

        public static string GetRelativeResourcePath(string resourceName)
        {
            return Path.Combine(Vendor, Name, "Resources", resourceName);
        }

        public static string GetAbsoluteResourcePath(string resourceName)
        {
            return Path.Combine(KSPUtil.ApplicationRootPath, "GameData", GetRelativeResourcePath(resourceName));
        }

        public static string GetSavePath(string fileName)
        {
            return Path.Combine(KSPUtil.ApplicationRootPath, "saves", HighLogic.SaveFolder, fileName);
        }

        public static GameObject GetPrefab(string assetName)
        {
            GameObject? obj = null;

            Utils.Log($"Loading prefab '{assetName}'");
            if (Prefabs != null)
            {
                Utils.Log("Prefabs available, loading asset from it.");
                obj = Prefabs.LoadAsset(assetName) as GameObject;
            }

            if (obj == null)
            {
                throw new IOException($"Could not load prefab '{assetName}' from asset bundle");
            }
            return obj;
        }
    }
}
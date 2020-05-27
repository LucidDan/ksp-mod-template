using System.Collections.Generic;
using System.IO;
using KSPAssets;
using KSPAssets.Editor;
using UnityEditor;
using UnityEngine;

public class AssetBundleBuilder
{
    // FIXME: at some point we should get vendor and mod name from the netkan file
    // These should map to the git username and repository project name resp.
    const string _vendorName = "LucidDan";
    const string _modName = "SampleMod";

    const string bundleDestination = "Resources";
    const string bundleExtension = "bundle";

    private static string ModVendor => _vendorName;
    private static string ModName => _modName;

    private static string CreateTempDLL(BundleDefinition bundle, AssetDefinition assetDefinition)
    {
        byte[] bytes = File.ReadAllBytes(assetDefinition.path);
        string path = assetDefinition.path.Substring(0, assetDefinition.path.Length - 3) + "bytes";
        Debug.Log($"AssetCompiler: Creating dll text asset at '{path}'");
        File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();
        return path;
    }
    private static IEnumerable<string> GetDependencyBundles(string bundleName, List<string> assets)
    {
        string[] dependencies = AssetDatabase.GetDependencies(assets.ToArray());
        List<string> assetNameList = new List<string>();
        foreach (string t in dependencies)
        {
            AssetImporter atPath = AssetImporter.GetAtPath(t);
            if (atPath == null) continue;

            string assetName = atPath.assetBundleName;

            if (string.IsNullOrEmpty(assetName))
                assetName = GetDependencyDirectoryBundle(t);

            if (bundleName != assetName && !string.IsNullOrEmpty(assetName) && !assetNameList.Contains(assetName))
                assetNameList.Add(assetName);
        }
        return assetNameList.ToArray();
    }

    private static string GetDependencyDirectoryBundle(string path)
    {
        while (true)
        {
            if (path == "Assets") return null;
            AssetImporter atPath = AssetImporter.GetAtPath(path);
            if (atPath != null)
            {
                if (!string.IsNullOrEmpty(atPath.assetBundleName)) return atPath.assetBundleName;
                path = path.Substring(0, path.LastIndexOf('/'));
                continue;
            }

            Debug.Log("NULL");
            return null;
        }
    }

    private static AssetBundleBuild[] CustomCreateAssetBundleBuildMap(
        IEnumerable<string> abNames,
        ICollection<string> tempFilenames)
    {
        Log("Creating Asset Bundle BuildMaps");
        List<AssetBundleBuild> assetBundleBuildList = new List<AssetBundleBuild>();
        foreach (string abName in abNames)
        {
            string path = AssetCompiler.CreateXML(abName);
            Log($"Created XML file: {path}");
            BundleDefinition bundle = BundleDefinition.LoadFromFile(path);
            List<string> assets = new List<string>();
            foreach (AssetDefinition asset in bundle.assets)
            {
                // never going to use DLLs inside Bundles I think, prob could ditch this code
                if (asset.type == "dll")
                {
                    Log($"Adding DLL asset: {asset.path}");
                    string tempDll = CreateTempDLL(bundle, asset);
                    tempFilenames.Add(tempDll);
                    assets.Add(tempDll);
                }
                else
                {
                    Log($"Adding regular asset: {asset.path}");
                    assets.Add(asset.path);
                }
            }

            Log($"Adding bundle definition asset: {abName}");
            assets.Add(AssetCompiler.ApplicationXMLFilePath(abName));
            // Not completely sure we need dependencies?
            Log("Adding dependencies.");
            bundle.dependencyNames = new List<string>(GetDependencyBundles(bundle.name, assets));
            bundle.Save(path);
            AssetBundleBuild buildMap = new AssetBundleBuild {assetBundleName = abName, assetNames = assets.ToArray()};
            assetBundleBuildList.Add(buildMap);
        }

        return assetBundleBuildList.ToArray();
    }

    // A custom builder because we want to define our own output path
    // unfortunately, the KSP AssetCompiler makes it VERY hard to customise things so ultimately I just ended up
    // building my own almost from scratch
    public static bool CustomBuildAssetBundles()
    {
        string[] abNames = AssetDatabase.GetAllAssetBundleNames();
        // bundlePath for intermediary output
        string bundlePath = Path.Combine(Application.dataPath, "../AssetBundles");
        // outputPath for final location
        string outputPath = Path.Combine(Application.dataPath, $"../../GameData/{ModVendor}/{ModName}/{bundleDestination}");

        if (!Directory.Exists(bundlePath))
            Directory.CreateDirectory(bundlePath);

        Log($"CustomAssetCompiler: Building {abNames.Length} asset bundle(s) 111000111");
        Log($"Building bundles: {string.Join(",", abNames)} in {bundlePath}");

        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        DirectoryInfo directoryInfo = new DirectoryInfo(bundlePath);

        Log("Starting pipeline.");

        List<string> tempFilenames = new List<string>();
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(
            bundlePath,
            CustomCreateAssetBundleBuildMap(abNames, tempFilenames),
            BuildAssetBundleOptions.ForceRebuildAssetBundle, 
            BuildTarget.StandaloneWindows
        );
        if (manifest == null)
        {
            Debug.LogError("ERROR in pipeline. Aborting.");
            return false;
        }
        Log($"Finished pipeline. Resulting bundles in manifest: {string.Join(",", manifest.GetAllAssetBundles())}");

        Log("Deleting temporary bundle definition files");
        if (tempFilenames.Count > 0)
        {
            foreach (string tempFilename in tempFilenames)
                File.Delete(tempFilename);
            AssetDatabase.Refresh();
        }

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        Log("Renaming bundle files.");
        foreach (string abName in abNames)
        {
            Log($"Renaming bundle from {abName} to ");
            foreach (FileInfo bundleFile in directoryInfo.GetFiles(abName, SearchOption.TopDirectoryOnly))
            {
                string finalBundleFile = Path.Combine(outputPath, $"{bundleFile.Name}.{bundleExtension}");


                if (File.Exists(finalBundleFile))
                {
                    Log($"Deleting existing bundle: {finalBundleFile}");
                    File.Delete(finalBundleFile);
                }

                Log($"Renaming bundle to final name: {finalBundleFile}");
                File.Move(bundleFile.FullName, finalBundleFile);
            }
        }
        Log($"CustomAssetCompiler: Finished building {abNames.Length} asset bundles");
        return true;
    }

    public static void RunAssetBuild()
    {
        // Do all of our asset building
        // System.Environment.GetCommandLineArgs for any command line arg processing
        // This will build all asset bundles and compress the files. Note that it never adds "ksp" as extension (?)
        // We make a custom asset compiler so we can instruct it (above) to use the output GameData directory 
        Log("Starting asset builder");
        if (CustomBuildAssetBundles())
        {
            Log("Finished asset builder");
            EditorApplication.Exit(0);
        }
        else
        {
            EditorApplication.Exit(1);
        }
    }

    public static void RunTests()
    {
        // Do any unity tests that we've created. For now, there is none; just return OK
        // System.Environment.GetCommandLineArgs for any command line arg processing
        Log("There is no test runner set up yet.");
        EditorApplication.Exit(0);
    }

    private static void Log(string msg)
    {
        Debug.Log($"*** ASSET BUILDER *** {msg}");
    }
}

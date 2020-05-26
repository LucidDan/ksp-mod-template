﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using KSPAssets.Editor;

public class AssetBundleBuilder
{
    static void RunAssetBuild()
    {
        // Do all of our asset building
        // System.Environment.GetCommandLineArgs for any command line arg processing
        // This will build all asset bundles using an extension of ".ksp", and compressing the files.
        AssetCompiler.BuildAssetBundles(true, true, "ksp", null);
        // Afterward, we want to copy the resulting bundles from <projectroot>\unity\AssetBundles\*.ksp --> <projectroot>\GameData\<vendor>\<modname>\
        EditorApplication.Exit(0);
    }

    static void RunTests()
    {
        // Do any unity tests that we've created. For now, there is none; just return OK
        // System.Environment.GetCommandLineArgs for any command line arg processing
        EditorApplication.Exit(0);
    }
}

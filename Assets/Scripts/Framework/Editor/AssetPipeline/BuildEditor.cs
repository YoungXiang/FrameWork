using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class BuildEditor
{
    [MenuItem("FrameWork/Build/Build Assets")]
    public static void BuildAssets()
    {
        InitBuildManager();
        if (BuildManager.Instance.isDirty)
        {
            BuildManager.Instance.BuildAssets();
        }
    }

    [MenuItem("FrameWork/Build/Build Assets(For Simulation)")]
    public static void BuildAssetsForSimulation()
    {
        InitBuildManager();
        if (BuildManager.Instance.isDirty)
        {
            BuildManager.Instance.BuildAssetsForSimulation();
        }
    }

    static void InitBuildManager()
    {
        string outputPath = EditorPrefs.GetString("AssetBundleOutputPath", string.Empty);
        if (outputPath.Length <= 0)
        {
            outputPath = EditorUtility.OpenFolderPanel("Select Assetbundle output folder", Application.streamingAssetsPath, "");
        }
        BuildManager.Instance.SetAssetbundleOutputPath(outputPath);

        string buildRulePath = EditorPrefs.GetString("AssetBundleRulePath", string.Empty);
        if (buildRulePath.Length <= 0)
        {
            buildRulePath = EditorUtility.OpenFilePanel("Select build rule", Application.dataPath, ".rule");
        }
        BuildManager.Instance.LoadRules(buildRulePath);
    }
}

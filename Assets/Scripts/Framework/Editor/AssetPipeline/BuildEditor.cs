using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FrameWork
{
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

        [MenuItem("FrameWork/Build/Build Player")]
        public static void BuildPlayer()
        {
            InitBuildManager();

            string outputPath = EditorPrefs.GetString("PlayerOutputPath", string.Empty);
            if (outputPath.Length <= 0)
            {
                outputPath = EditorUtility.OpenFolderPanel("Select Player output folder", Application.streamingAssetsPath, "");
                if (!string.IsNullOrEmpty(outputPath)) EditorPrefs.SetString("PlayerOutputPath", outputPath);
            }
            BuildManager.Instance.SetPlayerOutputPath(outputPath);
            BuildManager.Instance.BuildPlayer();
        }
        
        [MenuItem("FrameWork/Build/Select Player output folder")]
        public static void ChangePlayerPath()
        {
            string outputPath = EditorUtility.OpenFolderPanel("Select Player output folder", Application.streamingAssetsPath, "");
            if (!string.IsNullOrEmpty(outputPath)) EditorPrefs.SetString("PlayerOutputPath", outputPath);
        }

        [MenuItem("FrameWork/Build/Select Assetbundle output folder")]
        public static void ChangeAssetBundlePath()
        {
            string outputPath = EditorUtility.OpenFolderPanel("Select Assetbundle output folder", Application.streamingAssetsPath, "");
            if (!string.IsNullOrEmpty(outputPath)) EditorPrefs.SetString("AssetBundleOutputPath", outputPath);
        }

        [MenuItem("FrameWork/Build/Select BuildRule")]
        public static void ChangeBuildRulePath()
        {
            string buildRulePath = EditorUtility.OpenFilePanel("Select build rule", Application.dataPath, "json");
            if (!string.IsNullOrEmpty(buildRulePath)) EditorPrefs.SetString("AssetBundleRulePath", buildRulePath);
        }

        static void InitBuildManager()
        {
            string outputPath = EditorPrefs.GetString("AssetBundleOutputPath", string.Empty);
            if (outputPath.Length <= 0)
            {
                outputPath = EditorUtility.OpenFolderPanel("Select Assetbundle output folder", Application.streamingAssetsPath, "");
                if (!string.IsNullOrEmpty(outputPath)) EditorPrefs.SetString("AssetBundleOutputPath", outputPath);
            }
            BuildManager.Instance.SetAssetbundleOutputPath(outputPath);

            string buildRulePath = EditorPrefs.GetString("AssetBundleRulePath", string.Empty);
            if (buildRulePath.Length <= 0)
            {
                buildRulePath = EditorUtility.OpenFilePanel("Select build rule", Application.dataPath, "json");
                if (!string.IsNullOrEmpty(buildRulePath)) EditorPrefs.SetString("AssetBundleRulePath", buildRulePath);
            }
            BuildManager.Instance.LoadRules(buildRulePath);
        }
    }
}

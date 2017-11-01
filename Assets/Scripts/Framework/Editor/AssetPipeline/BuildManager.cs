using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildManager : Singleton<BuildManager>
{
    public BuildRuleParser parser;
    public string assetBundleOutputPath;
    public string serverAssetBundleOutputPath = "AssetBundleServer/";

    public bool isReady
    {
        get { return !isDirty && (parser != null && parser.parsedList.Count > 0); }
    }
    
    /// <summary>
    /// Is assets dirty or not? If true, then assets need rebuild
    /// </summary>
    public bool isDirty;
    public void SetDirty()
    {
        isDirty = true;
    }

    public void SetAssetbundleOutputPath(string abOutPath)
    {
        assetBundleOutputPath = abOutPath;
    }

    public void LoadRules(string path)
    {
        parser = new BuildRuleParser();
        parser.Parse(path);
    }

    public void BuildAssets()
    {
        PreBuildAsset();

        // Build Assets
        AssetBundleBuild[] buildMap = new AssetBundleBuild[parser.parsedList.Count];
        for (int i = 0; i < buildMap.Length; i++)
        {
            buildMap[i].assetBundleName = parser.parsedList[i].assetBundleName;
            buildMap[i].assetNames = parser.parsedList[i].assetFiles;
        }

        BuildPipeline.BuildAssetBundles(assetBundleOutputPath, buildMap, 
            BuildAssetBundleOptions.ChunkBasedCompression, 
            GetBuildTarget());

        PostBuildAsset();
        isDirty = false;
    }

    public void BuildAssetsForSimulation()
    {
        isDirty = false;
    }

    public void BuildPlayer()
    {

    }

    #region Privates
    void PreBuildAsset()
    {

    }

    void PostBuildAsset()
    {

    }
    #endregion

    public static BuildTarget GetBuildTarget()
    {
#if UNITY_ANDROID
        return BuildTarget.Android;
#elif UNITY_IOS
        return BuildTarget.iOS;
#elif UNITY_STANDALONE
        return BuildTarget.StandaloneWindows64;
#elif UNITY_STANDALONE_OSX
        return BuildTarget.StandaloneOSXIntel64;
#else
        return BuildTarget.StandaloneWindows64;	
#endif
    }
}

using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FrameWork
{
    public class BuildManager : Singleton<BuildManager>
    {
        public BuildRuleParser parser;
        public string assetBundleOutputPath;
        
        public string serverAssetBundleOutputPath = "AssetBundleServer/";

        public string playerOutputPath;

        public bool isReady
        {
            get { return !isDirty && (parser != null && parser.parsedList.Count > 0); }
        }

        /// <summary>
        /// Is assets dirty or not? If true, then assets need rebuild
        /// </summary>
        public bool isDirty
        {
            get
            {
                return EditorPrefs.GetBool("AssetDirty", false);
            }
        }

        public void SetDirty(bool dirty)
        {
            EditorPrefs.SetBool("AssetDirty", dirty);
        }
        
        public void SetAssetbundleOutputPath(string abOutPath)
        {
            assetBundleOutputPath = Path.Combine(abOutPath, AssetUtils.GetPlatformStringShort());
        }

        public void SetPlayerOutputPath(string pOutPath)
        {
            playerOutputPath = pOutPath;
        }

        public void LoadRules(string path)
        {
            parser = new BuildRuleParser();
            parser.Parse(path);
        }

        public bool BuildAssets()
        {
            LogUtil.LogColor(LogUtil.Color.yellow, "[Build] Build for target platform = {0}", EditorUserBuildSettings.activeBuildTarget.ToString());

            AssetBundleManifestNew manifest = new AssetBundleManifestNew();
            PreBuildAsset(manifest);

            // Build Assets
            AssetBundleBuild[] buildMap = new AssetBundleBuild[parser.parsedList.Count];
            for (int i = 0; i < buildMap.Length; i++)
            {
                buildMap[i].assetBundleName = parser.parsedList[i].assetBundleName;
                buildMap[i].assetNames = parser.parsedList[i].assetFiles;
            }

            AssetBundleManifest unityManifest = BuildPipeline.BuildAssetBundles(assetBundleOutputPath, buildMap,
                BuildAssetBundleOptions.ChunkBasedCompression,
                EditorUserBuildSettings.activeBuildTarget);

            if (unityManifest != null)
            {
                PostBuildAsset(manifest, unityManifest);
                SetDirty(false);
                return true;
            }

            return false;
        }

        public bool BuildAssetsForSimulation()
        {
            AssetBundleManifestNew manifest = new AssetBundleManifestNew();
            PreBuildAsset(manifest);
            PostBuildAssetSimulation(manifest);
            SetDirty(false);

            return true;
        }

        public bool BuildPlayer()
        {
            string outputPath = Path.Combine(Path.Combine(playerOutputPath, AssetUtils.GetPlatformStringShort()), GetPlatformBuildName());

            List<string> levels = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
            {
                levels.Add(EditorBuildSettings.scenes[i].path);
            }

            BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
            string errorMsg = BuildPipeline.BuildPlayer(levels.ToArray(), outputPath, EditorUserBuildSettings.activeBuildTarget, option);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                return false;
            }

            if (File.Exists(outputPath))
            {
                System.Diagnostics.Process.Start("explorer.exe", string.Format("/select,{0}", outputPath));
            }

            return true;
        }

        #region Privates
        /// <summary>
        /// Create the manifest
        /// </summary>
        void PreBuildAsset(AssetBundleManifestNew manifest)
        {
            for (int i = 0; i < parser.parsedList.Count; i++)
            {
                BuildRuleParser.Parsed parsed = parser.parsedList[i];
                int bundleHash = parsed.assetBundleName.GetHashCode();

                if (manifest.assetBundleConfig.ContainsKey(bundleHash)) continue;

                AssetBundleConfig abConf = new AssetBundleConfig();
                abConf.bundlePath = parsed.assetBundleName;
                abConf.hashCode = bundleHash;
                abConf.isBuiltIn = parsed.isBuiltIn;

                manifest.assetBundleConfig.Add(bundleHash, abConf);
            }
        }

        /// <summary>
        /// Post build
        /// 1. Encrypt 
        /// 2. Md5 comparison (ignored here : because no server is needed)
        /// 3. copy to server path (ignored here : because no server is needed)
        /// </summary>
        void PostBuildAsset(AssetBundleManifestNew manifest, AssetBundleManifest unityManifest)
        {
#if UNITY_2017
            Caching.ClearCache();
#else 
            Caching.CleanCache();
#endif
            AssetDatabase.RemoveUnusedAssetBundleNames();

            AssetBundleManifestNew serverManifest = IOUtils.DeserializeObjectFromFile<AssetBundleManifestNew>(serverAssetBundleOutputPath + "AssetManifest.manifest");

            int current = 0;
            bool hasServerAssetBundle = false;
            foreach (int bundleHash in manifest.assetBundleConfig.Keys)
            {
                AssetBundleConfig conf = manifest.assetBundleConfig[bundleHash];
                string bundleFullPath = Path.Combine(assetBundleOutputPath, conf.bundlePath);

                EditorUtility.DisplayProgressBar("Post build", conf.bundlePath, (float)(current/manifest.assetBundleConfig.Count));
                current++;

                byte[] encryptedBytes = IOUtils.EncryptBytes(IOUtils.LoadBytesFromFile(bundleFullPath));
                conf.md5 = IOUtils.ComputeMD5(encryptedBytes);

                string[] strDependencies = unityManifest.GetDirectDependencies(conf.bundlePath);
                if (strDependencies.Length > 0)
                {
                    conf.dependencies = new int[strDependencies.Length];
                    for (int i = 0; i < strDependencies.Length; i++)
                    {
                        int bundleDepenHash = strDependencies[i].GetHashCode();
                        conf.dependencies[i] = bundleDepenHash;
                    }
                }

                // compare with server
                if (!conf.isBuiltIn)
                {
                    hasServerAssetBundle = true;
                    string bundleServerPath = serverAssetBundleOutputPath + conf.bundlePath;
                    if (!File.Exists(bundleServerPath) || serverManifest == null)
                    {
                        conf.version = 0;
                    }
                    else
                    {
                        byte[] serverEncryptedBytes = IOUtils.EncryptBytes(IOUtils.LoadBytesFromFile(bundleServerPath));
                        string md5 = IOUtils.ComputeMD5(serverEncryptedBytes);
                        if (!md5.Equals(conf.md5))
                        {
                            AssetBundleConfig oldConfig = serverManifest.GetBundleConfig(bundleHash);
                            if (oldConfig == null)
                            {
                                conf.version = 0;
                            }
                            else
                            {
                                int cachedVersion = oldConfig.version;
                                conf.version = cachedVersion + 1;
                            }
                        }
                    }

                    // move bundle to new path
                    IOUtils.DeleteFileIfExists(bundleServerPath);
                    IOUtils.CreateDirectoryIfNotExist(Path.GetDirectoryName(bundleServerPath));
                    File.Move(bundleFullPath, bundleServerPath);
                    IOUtils.DeleteFileIfExists(bundleFullPath);
                }
                else
                {
                    conf.version = 0;
                }
            }

            if (hasServerAssetBundle)
            {
                // delete empty directories
                RemoveEmptyDirectories(assetBundleOutputPath);
            }

            for (int i = 0; i < parser.parsedList.Count; i++)
            {
                EditorUtility.DisplayProgressBar("Post build", "Finalizing", ((float)i / (float)parser.parsedList.Count));

                BuildRuleParser.Parsed parsed = parser.parsedList[i];
                int bundleHash = parsed.assetBundleName.GetHashCode();

                if (!manifest.assetBundleConfig.ContainsKey(bundleHash)) continue;

                for (int a = 0; a < parsed.assetFiles.Length; a++)
                {
                    manifest.assetsInBundle.Add(parsed.assetFiles[a], bundleHash);
                }
            }

            IOUtils.SerializeObjectToFile(Path.Combine(assetBundleOutputPath, "AssetManifest.manifest"), manifest);

            // for debugging
            string txtPath = Path.Combine(assetBundleOutputPath, "AssetManifest.txt");
            if (File.Exists(txtPath)) File.Delete(txtPath);
            IOUtils.CreateDirectoryIfNotExist(Path.GetDirectoryName(txtPath));
            File.WriteAllText(txtPath, manifest.ToString());

            EditorUtility.ClearProgressBar();

            LogUtil.LogColor(LogUtil.Color.yellow, "[Build] : Success!");
            #region Obsoleted 
            /*
            AssetBundleConfig abConf = manifest.assetBundleConfig[bundleHash];
            byte[] encryptedBytes = AssetUtils.EncryptBytes(AssetUtils.LoadSerializable(bundleFullPath));
            abConf.md5 = AssetUtils.ComputeMD5(encryptedBytes);
            
            AssetBundle resManifest = AssetBundle.LoadFromFile(bundlePrePath + "/Res");
            if (resManifest == null)
            {
                Debug.Log("Manifest here is null : " + bundlePrePath);
                return;
            }

            AssetBundleManifest bundleManifest = resManifest.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (bundleManifest == null)
            {
                Debug.LogError("AssetManifest not exist: " + bundleFullPath);
                resManifest.Unload(true);
                return;
            }
            string[] strDependencies = bundleManifest.GetDirectDependencies(bundleName);
            if (strDependencies.Length > 0)
            {
                abConf.dependencies = new int[strDependencies.Length];
                for (int i = 0; i < strDependencies.Length; i++)
                {
                    int bundleDepHash = strDependencies[i].GetHashCode();
                    abConf.dependencies[i] = bundleDepHash;
                }
            }
            resManifest.Unload(true);
            Object.DestroyImmediate(resManifest);

            // asset to bundle
            AssetBundle bundle = AssetBundle.LoadFromFile(bundleFullPath);
            string[] assetNames = bundle.GetAllAssetNames();
            foreach (string assetName in assetNames)
            {
                //Debug.LogFormat("AssetName = {0}, BundleName = {1}", assetName, bundleName);
                if (manifest.assetsInBundle.ContainsKey(assetName))
                {
                    Debug.LogFormat("[Warning]: Duplicated Asset detected: {0} appears to be in both {1} and {2}.", assetName, manifest.GetBundleConfig(manifest.assetsInBundle[assetName]).bundlePath,
                        bundleName);
                }
                else
                {
                    manifest.assetsInBundle.Add(assetName, bundleHash);
                }
            }
            bundle.Unload(true);
            Object.DestroyImmediate(bundle);
            //Debug.LogFormat("UnLoad AssetBundle = {0}", bundleName);

            // compare with server
            if (!abConf.isBuiltIn)
            {
                string bundleServerPath = localAssetBundleToServerDir + bundleName;
                if (!File.Exists(bundleServerPath) || manifestServerCache == null)
                {
                    abConf.version = 0;
                }
                else
                {
                    byte[] serverEncryptedBytes = AssetUtils.EncryptBytes(AssetUtils.LoadSerializable(bundleServerPath));
                    string md5 = AssetUtils.ComputeMD5(serverEncryptedBytes);
                    if (!md5.Equals(abConf.md5))
                    {
                        AssetBundleConfig oldConfig = manifestServerCache.GetBundleConfig(bundleHash);
                        if (oldConfig == null)
                        {
                            abConf.version = 0;
                        }
                        else
                        {
                            int cachedVersion = oldConfig.version;
                            abConf.version = cachedVersion + 1;
                        }
                    }
                }

                // move bundle to new path
                ReplaceBundle(bundleFullPath, bundleServerPath);
            }
            else
            {
                abConf.version = 0;
            }
            */
            #endregion
        }

        void RemoveEmptyDirectories(string rootDirectory)
        {
            string[] directories = Directory.GetDirectories(rootDirectory, "*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < directories.Length; i++)
            {
                string[] subdirectories = Directory.GetDirectories(directories[i]);
                if (subdirectories.Length > 0)
                {
                    for (int s = 0; s < subdirectories.Length; s++)
                    {
                        RemoveEmptyDirectories(subdirectories[s]);
                    }
                }
                else
                {
                    if (Directory.GetFiles(directories[i]).Length <= 0)
                    {
                        Directory.Delete(directories[i]);
                    }
                }
            }
        }

        void PostBuildAssetSimulation(AssetBundleManifestNew manifest)
        {
            for (int i = 0; i < parser.parsedList.Count; i++)
            {
                BuildRuleParser.Parsed parsed = parser.parsedList[i];
                int bundleHash = parsed.assetBundleName.GetHashCode();

                if (!manifest.assetBundleConfig.ContainsKey(bundleHash)) continue;
                for (int j = 0; j < parsed.assetFiles.Length; j++)
                {
                    manifest.assetsInBundle.Add(parsed.assetFiles[j], bundleHash);
                }
            }

            IOUtils.SerializeObjectToFile(Path.Combine(assetBundleOutputPath, "AssetManifest.manifest"), manifest);

            // for debugging
            string txtPath = Path.Combine(assetBundleOutputPath, "AssetManifest.txt");
            if (File.Exists(txtPath)) File.Delete(txtPath);
            IOUtils.CreateDirectoryIfNotExist(Path.GetDirectoryName(txtPath));
            File.WriteAllText(txtPath, manifest.ToString());

            LogUtil.LogColor(LogUtil.Color.yellow, "[Build] : Success! (Simulation)");
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

        public static string GetPlatformBuildName()
        {
#if UNITY_IOS
        return string.Format("TKShow_IosProject_{0}", System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm"));
#elif UNITY_ANDROID
        return string.Format("趣美秀3D_Android_{0}.apk", System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm"));
#else
            return string.Format("趣美秀3D_Windows_{0}.exe", System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm"));
#endif
        }
    }
}

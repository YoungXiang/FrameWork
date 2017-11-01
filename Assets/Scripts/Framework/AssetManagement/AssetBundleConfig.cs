using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class AssetBundleReference
    {
        public AssetBundle bundle;
        public int refCount;

        public AssetBundleReference()
        {
            bundle = null;
            refCount = 0;
        }

        public void AddRef()
        {
            refCount++;
        }

        public void RemoveRef()
        {
            refCount--;
        }

        public void Unload()
        {
            bundle.Unload(true);
            Debug.Log("Asset bundle is unloaded.");
        }
    }

    [System.Serializable]
    internal class AssetBundleConfig
    {
        // or bundle name
        public string bundlePath;

        // hash code of bundlePath
        public int hashCode;

        // version of this asset bundle
        public int version;

        // md5 of this asset bundle
        public string md5;

        // is this bundle built-in streamingAssets folder
        public bool isBuiltIn;

        // dependencies
        public int[] dependencies;

        // should re-download or not
        [System.NonSerialized]
        public bool needUpdate;

        // is downloaded or not
        [System.NonSerialized]
        public bool downloaded;
    }

    [System.Serializable]
    internal class AssetBundleManifestNew
    {
        public Dictionary<int, AssetBundleConfig> assetBundleConfig;

        // Key: AssetPath, e.g. 
        public Dictionary<string, int> assetsInBundle;

        public AssetBundleManifestNew()
        {
            assetBundleConfig = new Dictionary<int, AssetBundleConfig>();
            assetsInBundle = new Dictionary<string, int>();
        }

        public int GetBundleHashByAssetPath(string assetPath)
        {
            string lower = assetPath.ToLower();
            if (assetsInBundle.ContainsKey(lower))
            {
                return assetsInBundle[lower];
            }

            Debug.LogErrorFormat("[Error] : {0} is not found in any bundle.", lower);

            return int.MaxValue;
        }

        public AssetBundleConfig GetBundleConfig(int bundleHash)
        {
            if (assetBundleConfig.ContainsKey(bundleHash))
            {
                return assetBundleConfig[bundleHash];
            }

            Debug.LogErrorFormat("[Error] : BundleHash = {0} has no config.", bundleHash);

            return null;
        }

        public bool IsAssetBundleBuiltIn(int bundleHash)
        {
            if (assetBundleConfig.ContainsKey(bundleHash))
            {
                return assetBundleConfig[bundleHash].isBuiltIn;
            }

            return false;
        }

        public bool IsAssetBundleNeedUpdate(int bundleHash)
        {
            if (assetBundleConfig.ContainsKey(bundleHash))
            {
                return assetBundleConfig[bundleHash].needUpdate;
            }

            return false;
        }

        public bool IsAssetBundleDownloaded(int bundleHash)
        {
            if (assetBundleConfig.ContainsKey(bundleHash))
            {
                return assetBundleConfig[bundleHash].downloaded;
            }

            return false;
        }

#if UNITY_EDITOR
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (KeyValuePair<int, AssetBundleConfig> pair in assetBundleConfig)
            {
                AssetBundleConfig conf = pair.Value;
                sb.Append(conf.bundlePath).Append(string.Format(",[Version<{0}>],", conf.version)).Append("dependency : ");
                if (conf.dependencies != null)
                {
                    for (int i = 0; i < conf.dependencies.Length; i++)
                    {
                        AssetBundleConfig confD = assetBundleConfig[conf.dependencies[i]];
                        sb.Append(confD.bundlePath).Append(", ");
                    }
                }
                sb.Append("\t\n");
            }

            foreach (KeyValuePair<string, int> pair in assetsInBundle)
            {
                AssetBundleConfig conf = assetBundleConfig[pair.Value];
                sb.Append(pair.Key).Append(", ").Append(conf.bundlePath).Append(", ").Append("\t\n");
            }

            return sb.ToString();
        }
#endif
    }

}

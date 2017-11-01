using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public partial class AssetLoader
    {
        #region AssetBundle reference Management
        // All loaded asset bundles
        // Key: BundleID, Value: Bundle Ref
        public Dictionary<int, AssetBundleReference> assetBundles;
        // instance that references an assetbundle
        public Dictionary<int, int> instance2BundleBind;
        #endregion

        // AssetBundle Manifest, This is the custom manifest
        internal AssetBundleManifestNew manifest;

        internal ILowLevelLoader loader;
        internal AsyncCoroutine coroutiner;

        public AssetLoader()
        {
            assetBundles = new Dictionary<int, AssetBundleReference>();
            instance2BundleBind = new Dictionary<int, int>();
        }

        internal void Init()
        {
            coroutiner = AsyncCoroutine.Create();

#if SIMULATION
            loader = new LowLevelLoader_Editor();
#elif UNITY_ANDROID
            loader = new LowLevelLoader_Android();
#elif UNITY_IOS
            loader = new LowLevelLoader_Android();          // seems there is no big difference within 
#else
            loader = new LowLevelLoader_Android();
#endif

            LoadManifest();
        }

        void LoadManifest()
        {
            manifest = new AssetBundleManifestNew();
            string url = AssetUtils.serverAssetBundlePath + "AssetManifest.manifest";
            string cached = Path.Combine(AssetUtils.localConfigSavePath, "AssetManifest.manifest");
            if (!File.Exists(cached))
            {
                string builtIn = Path.Combine(AssetUtils.builtInAssetBundlePath, "AssetManifest.manifest");
                manifest = IOUtils.DeserializeObjectFromFile<AssetBundleManifestNew>(builtIn);
            }
            else
            {
                manifest = IOUtils.DeserializeObjectFromFile<AssetBundleManifestNew>(cached);
            }

            {
                // update abConf info
                foreach (KeyValuePair<int, AssetBundleConfig> pair in manifest.assetBundleConfig)
                {
                    AssetBundleConfig bConf = manifest.assetBundleConfig[pair.Key];
                    if (!bConf.isBuiltIn)
                    {
                        bConf.downloaded = ACaching.IsVersionCached(AssetUtils.serverAssetBundlePath + pair.Value.bundlePath, pair.Value.version);
                        bConf.needUpdate = !pair.Value.downloaded;
                    }
                    else
                    {
                        bConf.downloaded = true;
                        bConf.needUpdate = false;
                    }
                }

                EventDispatcher.DispatchEvent("AssetBundleManifest_Ready");
            }
            /*
            CreateDownloadRequest(url, (byte[] data) =>
            {
                // reload & save
                manifest = AssetUtils.DeserializeObject<AssetBundleManifestNew>(data);
                AssetUtils.SaveSerializable(cached, data);
                Debug.Log("AssetManifest downloaded and saved: " + cached);
                // update abConf info
                foreach (KeyValuePair<int, AssetBundleConfig> pair in manifest.assetBundleConfig)
                {
                    AssetBundleConfig bConf = manifest.assetBundleConfig[pair.Key];
                    if (!bConf.isBuiltIn)
                    {
                        bConf.downloaded = ACaching.IsVersionCached(AssetUtils.serverAssetBundlePath + pair.Value.bundlePath, pair.Value.version);
                        bConf.needUpdate = !pair.Value.downloaded;
                    }
                    else
                    {
                        bConf.downloaded = true;
                        bConf.needUpdate = false;
                    }
                }

                EventDispatcher.DispatchEvent("AssetBundleManifest_Ready");
            });
            */
        }

        #region Load Bundle
        void TryLoadBundle(int bundleHash, bool asyncLoad = false)
        {
            if (assetBundles.ContainsKey(bundleHash))
            {
                // bundle is Loaded
                return;
            }

            AssetBundleConfig bConf = manifest.GetBundleConfig(bundleHash);
            if (bConf.isBuiltIn)
            {
                if (asyncLoad) LoadBundleAsync(bConf);
                else LoadBundle(bConf);
            }
            else
            {
                LoadBundleWWWOrCache(bConf);
            }
        }
        
        // Load(AssetBundleConfig) => AssetBundleReference; then Add Reference count to AssetBundleReference
        void LoadBundle(AssetBundleConfig bConf)
        {
            AssetBundleReference bundleRef = loader.LoadBundleInternal(bConf);
            assetBundles.Add(bConf.hashCode, bundleRef);

            // Load Dependencies
            if (bConf.dependencies != null && bConf.dependencies.Length > 0)
            {
                for (int i = 0; i < bConf.dependencies.Length; i++)
                {
                    TryLoadBundle(bConf.dependencies[i]);
                    // bundle dependency reference
                    //assetBundles[bConf.dependencies[i]].AddRef();
                }
            }
        }

        // Load built-in assetbundle async
        void LoadBundleAsync(AssetBundleConfig bConf)
        {
            AssetBundleReference bundleRef = new AssetBundleReference();
            assetBundles.Add(bConf.hashCode, bundleRef);

            coroutiner.WaitForLoad(loader.LoadBundleAsyncInternal(bConf, (AssetBundle bundle) =>
            {
                if (bundleRef != null) bundleRef.bundle = bundle;
            }));

            // Load Dependencies
            if (bConf.dependencies != null && bConf.dependencies.Length > 0)
            {
                for (int i = 0; i < bConf.dependencies.Length; i++)
                {
                    TryLoadBundle(bConf.dependencies[i]);
                    //assetBundles[bConf.dependencies[i]].AddRef();
                }
            }
        }

        void LoadBundleWWWOrCache(AssetBundleConfig bConf)
        {
            AssetBundleReference bundleRef = new AssetBundleReference();
            assetBundles.Add(bConf.hashCode, bundleRef);
            
            coroutiner.WaitForLoad(loader.LoadBundleWWWOrCacheInternal(bConf, (AssetBundle bundle) =>
            {
                bConf.downloaded = ACaching.IsVersionCached(AssetUtils.serverAssetBundlePath + bConf.bundlePath, bConf.version);
                bConf.needUpdate = !bConf.downloaded;

                LogUtil.LogColor(LogUtil.Color.yellow, "[AssetsManagement]:Bundle load or download callback : {0}, version {1}, path {2}, bundle {3}", bConf.downloaded, bConf.version, bConf.bundlePath, bundle);
                if (bundleRef != null) bundleRef.bundle = bundle;
            }));

            // Load Dependencies
            if (bConf.dependencies != null && bConf.dependencies.Length > 0)
            {
                for (int i = 0; i < bConf.dependencies.Length; i++)
                {
                    TryLoadBundle(bConf.dependencies[i]);
                    //assetBundles[bConf.dependencies[i]].AddRef();
                }
            }
        }
        #endregion

        #region Load Asset Internal
        internal T LoadAsset<T>(string assetPath) where T : Object
        {
            coroutiner.NewLoadingQueue();
            // 1. Load bundle first
            int bundleHash = manifest.GetBundleHashByAssetPath(assetPath);
            TryLoadBundle(bundleHash);

            AssetBundleReference abRef = assetBundles[bundleHash];
            if (abRef.bundle == null)
            {
                Debug.LogErrorFormat("Trying to load a none built-in asset sync. Please use LoadFromWWWOrCache instead.");
                return null;
            }
#if SIMULATION
            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
#else
            T asset = abRef.bundle.LoadAsset<T>(assetPath);
#endif

            // Add the bundle reference here.
            BindAssetToAssetBundleRef(asset.GetInstanceID(), bundleHash);

            return asset;
        }

        internal void LoadAssetAsync<T>(string assetPath, AssetAsyncCallback<T> callback) where T : Object
        {
            coroutiner.NewLoadingQueue();

            int bundleHash = manifest.GetBundleHashByAssetPath(assetPath);
            TryLoadBundle(bundleHash, true);

            AssetBundleReference abRef = assetBundles[bundleHash];
            if (abRef.bundle == null)
            {
                // create a wait 
                WaitForAssetBundleComplete wait = new WaitForAssetBundleComplete(abRef);
                coroutiner.WaitForLoad(wait);
            }
            coroutiner.WaitForLoad(loader.LoadAssetAsyncInternal(abRef, assetPath, (T asset) =>
            {
                if (asset != null)
                {
                    // Add the bundle reference here.
                    BindAssetToAssetBundleRef(asset.GetInstanceID(), bundleHash);
                    callback(asset);
                }
            }));
        }

        internal void LoadAssetWWWOrCache<T>(string assetPath, AssetAsyncCallback<T> callback) where T : Object
        {
            coroutiner.NewLoadingQueue();

            int bundleHash = manifest.GetBundleHashByAssetPath(assetPath);
            TryLoadBundle(bundleHash, true);

            AssetBundleReference abRef = assetBundles[bundleHash];
            if (abRef.bundle == null)
            {
                // create a wait 
                WaitForAssetBundleComplete wait = new WaitForAssetBundleComplete(abRef);
                coroutiner.WaitForLoad(wait);
            }
            coroutiner.WaitForLoad(loader.LoadAssetAsyncInternal(abRef, assetPath, (T asset) =>
            {
                if (asset != null)
                {
                    BindAssetToAssetBundleRef(asset.GetInstanceID(), bundleHash);
                    callback(asset);
                }
            }));
        }

        internal AsyncOperation LoadSceneAysnc(string scenePath)
        {
            coroutiner.NewLoadingQueue();
            int bundleHash = manifest.GetBundleHashByAssetPath(scenePath);
            TryLoadBundle(bundleHash, true);

            AssetBundleReference abRef = assetBundles[bundleHash];
            if (abRef.bundle == null)
            {
                // create a wait 
                WaitForAssetBundleComplete wait = new WaitForAssetBundleComplete(abRef);
                coroutiner.WaitForLoad(wait);
            }

            // there is no asset to load, so no asset to bind, just add the ref
            AddBundleRefInternal(bundleHash);
            SceneAsyncRequest sar = loader.LoadSceneAsyncInternal(abRef, scenePath) as SceneAsyncRequest;
            coroutiner.WaitForLoad(sar);
            return sar.asyncOp;
        }

        internal void UnloadSceneAsync(string scenePath)
        {
            int bundleHash = manifest.GetBundleHashByAssetPath(scenePath);
            RemoveBundleRefInternal(bundleHash);
        }
        #endregion

        #region Ref & Un Ref
        // get bundle by asset instanceID
        internal int GetBundleHash(int instanceID)
        {
            if (instance2BundleBind.ContainsKey(instanceID))
            {
                return instance2BundleBind[instanceID];
            }

            return int.MaxValue;
        }

        internal void BindAssetToAssetBundleRef(int instanceID, int bundleHash)
        {
            if (assetBundles.ContainsKey(bundleHash))
            {
                // record asset instance - asset bundle 
                if (!instance2BundleBind.ContainsKey(instanceID))
                    instance2BundleBind.Add(instanceID, bundleHash);

                // add ref
                AddBundleRefInternal(bundleHash);
            }
        }
        
        // Remove Assetbundle ref by asset instanceID
        internal void RemoveAssetRefToAssetBundle(int instanceID)
        {
            int bundleHash = GetBundleHash(instanceID);
            RemoveBundleRefInternal(bundleHash);
        }

        internal void AddBundleRefInternal(int bundleHash)
        {
            AssetBundleReference abRef = assetBundles[bundleHash];
            abRef.AddRef();

            // add dependencies references
            AssetBundleConfig bConf = manifest.GetBundleConfig(bundleHash);
            if (bConf != null && bConf.dependencies != null && bConf.dependencies.Length > 0)
            {
                for (int i = 0; i < bConf.dependencies.Length; i++)
                {
                    AddBundleRefInternal(bConf.dependencies[i]);
                }
            }
        }

        internal void RemoveBundleRefInternal(int bundleHash)
        {
            if (assetBundles.ContainsKey(bundleHash))
            {
                AssetBundleReference abRef = assetBundles[bundleHash];
                abRef.RemoveRef();

                if (abRef.refCount <= 0)
                {
                    abRef.Unload();
                    assetBundles.Remove(bundleHash);    // this bundle is now released
                }

                // remove dependencies references
                AssetBundleConfig bConf = manifest.GetBundleConfig(bundleHash);
                if (bConf != null && bConf.dependencies != null && bConf.dependencies.Length > 0)
                {
                    for (int i = 0; i < bConf.dependencies.Length; i++)
                    {
                        RemoveBundleRefInternal(bConf.dependencies[i]);
                    }
                }
            }
        }
        #endregion       
    }
}
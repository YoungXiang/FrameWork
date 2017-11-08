using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FrameWork
{
    internal class LowLevelLoader_Editor : ILowLevelLoader
    {
        readonly string fakeBundlePath = "Assets/Scripts/Framework/AssetManagement/LowLevelLoader/fake.unity3d";
        public static AssetBundle fakeBundle;
        void TryUnloadFakeBundle()
        {
            if (fakeBundle != null)
            {
                fakeBundle.Unload(true);
            }
        }

        #region Bundle Load
        public AssetBundleReference LoadBundleInternal(AssetBundleConfig config)
        {
            Debug.Log("[Loader]: Loading AssetBundle : " + config.bundlePath);

            string bundlePath = Path.Combine(AssetUtils.builtInAssetBundlePath, config.bundlePath);

            AssetBundleReference abRef = new AssetBundleReference();
            if (File.Exists(bundlePath))
            {
                abRef.bundle = AssetBundle.LoadFromFile(bundlePath);
            }
            else
            {
                Debug.Log("[Loader]: Bundle not exists[SimulationMode]");
                TryUnloadFakeBundle();
                abRef.bundle = AssetBundle.LoadFromFile(fakeBundlePath);
                fakeBundle = abRef.bundle;
            }

            return abRef; 
        }

        public IAsyncRequestBase LoadBundleAsyncInternal(AssetBundleConfig config, AssetBundleAsyncCallback callback)
        {
            AssetBundleAsyncRequest request = new AssetBundleAsyncRequest();
            request.id = config.hashCode;
            request.beginRequest = () =>
            {
                string bundlePath = Path.Combine(AssetUtils.builtInAssetBundlePath, config.bundlePath);
                Debug.Log("[Loader]: Loading AssetBundle Async : " + config.bundlePath);
                if (File.Exists(bundlePath))
                {
                    request.asyncOp = AssetBundle.LoadFromFileAsync(bundlePath);
                }
                else
                {
                    TryUnloadFakeBundle();
                    Debug.Log("[Loader]: Bundle not exists[SimulationMode]");
                    request.asyncOp = AssetBundle.LoadFromFileAsync(fakeBundlePath);
                }
            };
            request.endRequest = () =>
            {
                fakeBundle = request.asyncOp.assetBundle;
                callback(request.asyncOp.assetBundle);
            };

            return request;
        }

        public IAsyncRequestBase LoadBundleWWWOrCacheInternal(AssetBundleConfig config, AssetBundleAsyncCallback callback)
        {
            AdvancedABRequest request = new AdvancedABRequest();
            request.id = config.hashCode;
            request.beginRequest = () =>
            {
                //int version = config.needUpdate ? config.version + 1 : config.version;
                Debug.Log("[Loader]: Loading AssetBundle From WWW : " + AssetUtils.serverAssetBundlePath + config.bundlePath + "<" + config.version + ">");
                request.www = AWWW.LoadFromCacheOrDownload(AssetUtils.serverAssetBundlePath + config.bundlePath, config.version);
            };
            request.endRequest = () =>
            {
                if (request.www.error.Length <= 0)
                {
                    callback(request.cq.assetBundle);
                }
                request.www = null;
            };

            return request;
        }
        #endregion

        #region Asset Async
        public IAsyncRequestBase LoadAssetAsyncInternal<T>(AssetBundleReference abRef, string assetPath, AssetAsyncCallback<T> callback) where T : UnityEngine.Object
        {
            FakeAssetAsyncRequest request = new FakeAssetAsyncRequest();
            request.id = assetPath.GetHashCode();
            request.beginRequest += () => { };
            request.endRequest += () => {
                //callback(request.asyncOp.asset as T);
#if UNITY_EDITOR
                callback(UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath));
#endif
            };
            return request;           
        }
        #endregion

        #region Scene Load
        public IAsyncRequestBase LoadSceneAsyncInternal(AssetBundleReference abRef, string scenePath, bool unloadPrevious)
        {
            SceneAsyncRequest request = new SceneAsyncRequest();
            request.id = scenePath.GetHashCode();
            request.beginRequest += () => {
                request.asyncOp = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenePath, unloadPrevious ? LoadSceneMode.Single : LoadSceneMode.Additive);
                request.asyncOp.allowSceneActivation = false;
            };
            request.endRequest += () => 
            {
                request.asyncOp.allowSceneActivation = true;
            };
            
            return request;
        }
        #endregion
    }
}
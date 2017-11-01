using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    internal class LowLevelLoader_Editor : ILowLevelLoader
    {
        public AssetBundleReference LoadBundleInternal(AssetBundleConfig config)
        {
            Debug.Log("[Loader]: Loading AssetBundle : " + config.bundlePath);

            AssetBundleReference abRef = new AssetBundleReference();
            abRef.bundle = AssetBundle.LoadFromFile(AssetUtils.builtInAssetBundlePath + config.bundlePath);

            return abRef; 
        }

        public IAsyncRequestBase LoadBundleAsyncInternal(AssetBundleConfig config, AssetBundleAsyncCallback callback)
        {
            AssetBundleAsyncRequest request = new AssetBundleAsyncRequest();
            request.id = config.hashCode;
            request.beginRequest = () =>
            {
                Debug.Log("[Loader]: Loading AssetBundle Async : " + config.bundlePath);
                request.asyncOp = AssetBundle.LoadFromFileAsync(AssetUtils.builtInAssetBundlePath + config.bundlePath);
            };
            request.endRequest = () => 
            {
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
    }
}
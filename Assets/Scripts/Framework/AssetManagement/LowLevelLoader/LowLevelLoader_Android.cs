using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    internal class LowLevelLoader_Android : ILowLevelLoader
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
            Debug.Log("[Loader]: Loading AssetBundle Async : " + config.bundlePath);

            AssetBundleAsyncRequest request = new AssetBundleAsyncRequest();
            request.id = config.hashCode;
            request.beginRequest = () => { request.asyncOp = AssetBundle.LoadFromFileAsync(AssetUtils.builtInAssetBundlePath + config.bundlePath); };
            request.endRequest = () => { callback(request.asyncOp.assetBundle); };

            return request;
        }

        public IAsyncRequestBase LoadBundleWWWOrCacheInternal(AssetBundleConfig config, AssetBundleAsyncCallback callback)
        {
            Debug.Log("[Loader]: Loading AssetBundle From WWW : " + AssetUtils.serverAssetBundlePath + config.bundlePath);

            AdvancedABRequest request = new AdvancedABRequest();
            request.id = config.hashCode;
            request.beginRequest = () => 
            {
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

        #region Asset 
        public IAsyncRequestBase LoadAssetAsyncInternal<T>(AssetBundleReference abRef, string assetPath, AssetAsyncCallback<T> callback) where T : UnityEngine.Object
        {
            AssetAsyncRequest request = new AssetAsyncRequest();
            request.id = assetPath.GetHashCode();
            request.beginRequest += () => {
                request.asyncOp = abRef.bundle.LoadAssetAsync<T>(assetPath);
            };
            request.endRequest += () => { callback(request.asyncOp.asset as T); };
            return request;           
        }
        #endregion
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    internal interface ILowLevelLoader
    {
        #region Bundle Load
        // 同步加载
        AssetBundleReference LoadBundleInternal(AssetBundleConfig config);
        // 异步加载
        IAsyncRequestBase LoadBundleAsyncInternal(AssetBundleConfig config, AssetBundleAsyncCallback callback);
        // 网络加载
        IAsyncRequestBase LoadBundleWWWOrCacheInternal(AssetBundleConfig config, AssetBundleAsyncCallback callback);
        #endregion

        #region Asset async Load
        IAsyncRequestBase LoadAssetAsyncInternal<T>(AssetBundleReference abRef, string assetPath, AssetAsyncCallback<T> callback) where T : UnityEngine.Object;
        //IAsyncRequestBase LoadAssetWWWOrCacheInternal<T>(AssetBundleReference abRef, string assetPath, AssetAsyncCallback<T> callback) where T : UnityEngine.Object;
        #endregion

        #region Scene load
        IAsyncRequestBase LoadSceneAsyncInternal(AssetBundleReference abRef, string scenePath);
        #endregion
    }
}
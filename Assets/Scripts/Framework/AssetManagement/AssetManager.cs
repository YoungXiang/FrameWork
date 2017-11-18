using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public static class AssetManager
    {
        internal static AssetLoader s_loader;
        static AssetManager()
        {
            IOUtils.CreateDirectoryIfNotExist(AssetUtils.localConfigSavePath);

            s_loader = new AssetLoader();
            s_loader.Init();
        }

        #region Public API
        public static void UnbindAsset(int assetInstanceID)
        {
            s_loader.RemoveAssetRefToAssetBundle(assetInstanceID);
        }

        public static T LoadAsset<T>(T obj, GameObject host, string assetPath) where T : Object
        {
            int oldInstanceID = -1;
            if (obj != null) oldInstanceID = obj.GetInstanceID();

            obj = s_loader.LoadAsset<T>(assetPath);

            if (oldInstanceID >= 0) UnbindAsset(oldInstanceID);
            host.BindAsset(obj);

            return obj;
        }

        public static void LoadAssetAsync<T>(T obj, GameObject host, string assetPath, AssetAsyncCallback<T> callback) where T : Object
        {
            int oldInstanceID = -1;
            if (obj != null) oldInstanceID = obj.GetInstanceID();

            s_loader.LoadAssetAsync(assetPath, (T res) =>
            {
                // in case host is destroyed
                if (host != null)
                {
                    host.BindAsset(res);
                    callback(res);
                }
                else
                {
                    UnbindAsset(res.GetInstanceID());
                }

                if (oldInstanceID >= 0) UnbindAsset(oldInstanceID);
            });
        }

        public static void LoadAssetWWWOrCache<T>(T obj, GameObject host, string assetPath, AssetAsyncCallback<T> callback) where T : Object
        {
            int oldInstanceID = -1;
            if (obj != null) oldInstanceID = obj.GetInstanceID();

            s_loader.LoadAssetWWWOrCache(assetPath, (T res) =>
            {
                // in case host is destroyed
                if (host != null)
                {
                    host.BindAsset(res);
                    callback(res);
                }
                else
                {
                    UnbindAsset(res.GetInstanceID());
                }

                if (oldInstanceID >= 0) UnbindAsset(oldInstanceID);
            });
        }

        /// <summary>
        /// Always destroy the previous before load another prefab instance.
        /// </summary>
        public static GameObject LoadPrefab(string assetPath)
        {
            GameObject prefab = s_loader.LoadAsset<GameObject>(assetPath);
            GameObject obj = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            obj.name = prefab.name;
            obj.BindAsset(prefab);
            return obj;
        }

        public static GameObject LoadPrefab(string assetPath, Transform parent)
        {
            GameObject prefab = s_loader.LoadAsset<GameObject>(assetPath);
            GameObject obj = Object.Instantiate(prefab, parent, false);
            obj.name = prefab.name;
            obj.BindAsset(prefab);
            return obj;
        }


        /// <summary>
        /// Always destroy the previous before load another prefab instance.
        /// </summary>
        public static void LoadPrefabAsync(string assetPath, AssetAsyncCallback<GameObject> callback)
        {
            s_loader.LoadAssetAsync(assetPath, (GameObject prefab) => 
            {
                GameObject obj = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
                obj.name = prefab.name;
                obj.BindAsset(prefab);
                callback(obj);
            });
        }

        /// <summary>
        /// Always destroy the previous before load another prefab instance.
        /// </summary>
        public static void LoadPrefabWWWOrCache(string assetPath, AssetAsyncCallback<GameObject> callback)
        {
            s_loader.LoadAssetWWWOrCache(assetPath, (GameObject prefab) =>
            {
                GameObject obj = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
                obj.name = prefab.name;
                obj.BindAsset(prefab);
                callback(obj);
            });
        }

        /// <summary>
        /// for scene loading, provide only async operations
        /// </summary>
        /// <param name="path"></param>
        /// <param name="unloadPrevious">if true then will unload the previous unity scene, this is used for loading -> main</param>
        public static AsyncOperation LoadScene(string path, bool unloadPrevious = false)
        {
            return s_loader.LoadSceneAysnc(path, unloadPrevious);
        }

        public static void UnloadScene(string path)
        {
            s_loader.UnloadSceneAsync(path);
        }

        /// Listener for async loading
        public static void AddListenerForAsset(string assetPath, AsyncRequestListener listener)
        {
            s_loader.coroutiner.AddListener(assetPath.GetHashCode(), listener);
        }
        #endregion
    }
}

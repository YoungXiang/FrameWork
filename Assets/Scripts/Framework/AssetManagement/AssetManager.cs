using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public static class AssetManager
    {
        public static AssetLoader s_loader;
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

        // for scene loading, provide only async operations
        public static AsyncOperation LoadScene(string path)
        {
            return s_loader.LoadSceneAysnc(path);
        }

        public static void UnloadScene(string path)
        {
            s_loader.UnloadSceneAsync(path);
        }
        #endregion
    }
}

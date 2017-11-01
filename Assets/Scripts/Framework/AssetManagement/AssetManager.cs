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
            s_loader.RemoveRefInternal(assetInstanceID);
        }

        public static T LoadAsset<T>(T obj, GameObject host, string assetPath) where T : Object
        {
            int oldInstanceID = -1;
            if (obj != null) oldInstanceID = obj.GetInstanceID();

            obj = s_loader.LoadAsset<T>(assetPath);

            if (oldInstanceID >= 0) s_loader.RemoveRefInternal(oldInstanceID);
            host.BindAsset(obj);

            return obj;
        }
        #endregion
    }
}

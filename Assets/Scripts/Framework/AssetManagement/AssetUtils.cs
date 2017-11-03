using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    // Delegates
    public delegate void VoidCallback();
    public delegate void DownloadCallback(byte[] data);
    public delegate void AssetBundleAsyncCallback(AssetBundle bundle);
    public delegate void AssetAsyncCallback<T>(T asset) where T : UnityEngine.Object;

    public delegate void AsyncRequestListener(IAsyncRequestBase op);


    public static class AssetUtils
    {
        #region 路径配置
        // 内置AB路径
        public static readonly string builtInAssetBundlePath = Application.streamingAssetsPath + "/AssetBundle/" + GetPlatformStringShort() + "/";
        // 配置下载路径
        public static readonly string localConfigSavePath = Path.Combine(Application.persistentDataPath, GetPlatformStringShort());

        // 服务器AB路径 将ip替换
        public static readonly string serverAssetBundlePath = string.Format("http://{0}/assets/", "192.168.0.125") + GetPlatformStringShort() + "/Encrypted/";
        #endregion

        public static string GetPlatformStringShort()
        {
#if UNITY_STANDALONE
            return "Windows";
#elif UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
            return "Default";
#endif
        }
    }
}
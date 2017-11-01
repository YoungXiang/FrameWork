using UnityEngine;

namespace FrameWork
{
    static class AssetLoaderExtend
    {
        public static void BindAsset(this GameObject go, Object asset)
        {
            go.GetOrCreateComponent<AssetRecorder>().BindAsset(asset);
        }
    }    
}
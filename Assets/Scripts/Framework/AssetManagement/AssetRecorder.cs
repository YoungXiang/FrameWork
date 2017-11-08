using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class AssetRecorder : MonoBehaviour
    {
        private List<int> assetBundleRefs = new List<int>();

        public int assetReferenceCount
        {
            get { return assetBundleRefs.Count; }
        }

        private void OnDestroy()
        {
            UnbindAllAssets();
        }

        public void BindAsset(Object asset)
        {
            int assetInstanceID = asset.GetInstanceID();
            assetBundleRefs.Add(assetInstanceID);
        }

        protected void UnbindAllAssets()
        {
            for (int i = 0; i < assetBundleRefs.Count; i++)
            {
                // do the unbind operation
                AssetManager.UnbindAsset(assetBundleRefs[i]);
            }
            assetBundleRefs.Clear();
        }
    }
}
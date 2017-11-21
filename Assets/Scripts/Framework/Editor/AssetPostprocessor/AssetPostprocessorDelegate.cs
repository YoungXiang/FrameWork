using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FrameWork
{
    public class AssetPostprocessorDelegate : AssetPostprocessor
    {

        #region Pre processors : where you can edit the assetImporter and affects the final results of assets.
        void OnPreprocessTexture()
        {
            LogUtil.LogColor(LogUtil.Color.gray, "OnPreprocessTexture:{0}", Path.GetFileName(assetPath));
        }
        void OnPreprocessModel()
        {
            LogUtil.LogColor(LogUtil.Color.gray, "OnPreprocessModel:{0}", Path.GetFileName(assetPath));
        }
        void OnPreprocessAnimation()
        {
            LogUtil.LogColor(LogUtil.Color.gray, "OnPreprocessAnimation:{0}", Path.GetFileName(assetPath));
        }
        void OnPreprocessAudio()
        {
            LogUtil.LogColor(LogUtil.Color.gray, "OnPreprocessAudio:{0}", Path.GetFileName(assetPath));
        }
        #endregion
        /*
        Material OnAssignMaterialModel(Material material, Renderer renderer)
        {
            return material;
        }
        */
        #region Post processors
        void OnPostprocessTexture(Texture2D texture)
        {
            LogUtil.LogColor(LogUtil.Color.gray, "OnPostprocessTexture:{0}", Path.GetFileName(assetPath));
        }

        void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
        {
            LogUtil.LogColor(LogUtil.Color.gray, "OnPostprocessSprites:{0}", Path.GetFileName(assetPath));
        }

        void OnPostprocessModel(GameObject g)
        {
            LogUtil.LogColor(LogUtil.Color.gray, "OnPostprocessModel:{0}", Path.GetFileName(assetPath));
        }

        void OnPostprocessAudio(AudioClip audio)
        {
            LogUtil.LogColor(LogUtil.Color.gray, "OnPostprocessAudio:{0}", Path.GetFileName(assetPath));
        }

        void OnPostprocessGameObjectWithUserProperties(
            GameObject go,
            string[] propNames,
            System.Object[] values)
        {
            for (int i = 0; i < propNames.Length; i++)
            {
                string propName = propNames[i];
                System.Object value = (System.Object)values[i];

                LogUtil.LogColor(LogUtil.Color.gray, "Propname: " + propName + " value: " + values[i]);

                if (value.GetType().ToString() == "System.Int32")
                {
                    int myInt = (int)value;
                    // do something useful
                }

                // etc...
            }
        }

        public void OnPostprocessAssetbundleNameChanged(string assetPath, string previousAssetBundleName, string newAssetBundleName)
        {
            LogUtil.LogColor(LogUtil.Color.gray, "Asset " + assetPath + " has been moved from assetBundle " + previousAssetBundleName + " to assetBundle " + newAssetBundleName + ".");
        }
        #endregion

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string str in importedAssets)
            {
                LogUtil.LogColor(LogUtil.Color.gray, "Reimported Asset: " + str);
            }
            foreach (string str in deletedAssets)
            {
                LogUtil.LogColor(LogUtil.Color.gray, "Deleted Asset: " + str);
            }

            for (int i = 0; i < movedAssets.Length; i++)
            {
                LogUtil.LogColor(LogUtil.Color.gray, "Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
            }
        }
    }
}

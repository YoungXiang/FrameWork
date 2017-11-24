using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class MenuItems_SceneTool_UIMaterialSync
{
    static string Mat_fast = "Assets/Res/UI/Common/Materials/UI-Fast.mat";
    static string Mat_def = "Assets/Res/UI/Common/Materials/UI-Default.mat";

    [MenuItem("SceneTool/UI Material Sync")]
    public static void UIMaterialSync()
    {
        Object[] objs = GameObject.FindObjectsOfType(typeof(GameObject));
        for (int i = 0; i < objs.Length; i++)
        {
            GameObject go = objs[i] as GameObject;
            bool isMasked = IsMasked(go);
            Text text = go.GetComponent<Text>();
            if (text != null)
            {
                text.material = AssetDatabase.LoadAssetAtPath<Material>(isMasked ? Mat_def : Mat_fast);
            }

            Image image = go.GetComponent<Image>();
            {
                if (image != null)
                {
                    image.material = AssetDatabase.LoadAssetAtPath<Material>(isMasked ? Mat_def : Mat_fast);
                }
            }
        }
    }

    public static bool IsMasked(GameObject go)
    {
        if (go.GetComponent<Mask>() == null)
        {
            return go.GetComponentInParent<Mask>() != null;
        }

        return true;
    }
    
}

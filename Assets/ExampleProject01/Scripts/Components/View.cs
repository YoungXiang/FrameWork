using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Artemis.Attributes.ArtemisComponentPool(
    InitialSize = ComponentDefine.ComponentPoolSize, 
    IsResizable = true, 
    IsSupportMultiThread = true, 
    ResizeSize = ComponentDefine.ResizeScale)]
public class View : Artemis.ComponentPoolable
{
    public GameObject gameObject;
    public Transform transform;
    public string assetPath;

    public View() { }

    public View(string assetPath_)
    {
        assetPath = assetPath_;
    }

    public void Load(string assetPath_)
    {
        assetPath = assetPath_;
        gameObject = FrameWork.PrefabPool.Instance.Instantiate(assetPath);
        transform = gameObject.transform;
    }

    public override void Initialize()
    {
        if (!string.IsNullOrEmpty(assetPath))
        {
            gameObject = FrameWork.PrefabPool.Instance.Instantiate(assetPath);
            transform = gameObject.transform;
        }
    }

    public override void CleanUp()
    {
        FrameWork.PrefabPool.Instance.Recycle(assetPath, gameObject);
    }
}

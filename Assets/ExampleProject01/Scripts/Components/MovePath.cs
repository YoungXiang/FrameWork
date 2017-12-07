using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Artemis.Attributes.ArtemisComponentPool(
    InitialSize = ComponentDefine.ComponentPoolSize,
    IsResizable = true,
    IsSupportMultiThread = true,
    ResizeSize = ComponentDefine.ResizeScale)]
public class MovePath : Artemis.ComponentPoolable
{
    public Vector3 dstPos;
    public List<Grid> path;
    public int currentIndex;
    public bool reached
    {
        get
        {
            return path.Count > 0 ? currentIndex >= path.Count : false;
        }
    }

    public MovePath()
    {
        path = new List<Grid>();
    }

    public MovePath(Vector3 dstPos_)
    {
        dstPos = dstPos_;
        path = new List<Grid>();
    }

    public void FindPath(Vector3 srcPos_, Vector3 dstPos_)
    {
        dstPos = dstPos_;
        currentIndex = 0;
        EntityWorldRegistry.Instance.gridWorld.FindPath(srcPos_, dstPos_, path);
    }

    public override void CleanUp()
    {
        path.Clear();
    }
}

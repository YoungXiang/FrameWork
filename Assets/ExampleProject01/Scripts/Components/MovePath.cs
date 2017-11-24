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

    public void SetDstPos(Vector3 dstPos_)
    {
        dstPos = dstPos_;
    }

    public override void CleanUp()
    {
        path.Clear();
    }
}

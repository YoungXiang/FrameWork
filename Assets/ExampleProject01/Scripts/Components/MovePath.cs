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
    public bool reached;

    public MovePath()
    {
        path = new List<Grid>();
        reached = false;
    }

    public MovePath(Vector3 dstPos_)
    {
        dstPos = dstPos_;
        path = new List<Grid>();
        reached = false;
    }

    public void SetDstPos(Vector3 dstPos_)
    {
        dstPos = dstPos_;
        reached = false;
    }

    public override void CleanUp()
    {
        path.Clear();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Artemis.Attributes.ArtemisComponentPool(
    InitialSize = ComponentDefine.ComponentPoolSize,
    IsResizable = true,
    IsSupportMultiThread = true,
    ResizeSize = ComponentDefine.ResizeScale)]
public class Velocity : Artemis.ComponentPoolable
{
    public Vector3 value;

    public Velocity() { }

    public Velocity(Vector3 val_) { value = val_; }
    
}

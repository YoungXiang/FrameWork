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
    public float value;

    public Velocity() { }

    public Velocity(float val_) { value = val_; }
    
}

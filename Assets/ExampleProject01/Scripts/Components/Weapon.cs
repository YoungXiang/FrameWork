using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Artemis.Attributes.ArtemisComponentPool(
    InitialSize = ComponentDefine.ComponentPoolSize,
    IsResizable = true,
    IsSupportMultiThread = true,
    ResizeSize = ComponentDefine.ResizeScale)]
public class Weapon : Artemis.ComponentPoolable
{
    public int activeWeapon;
    public Weapon() { }
    public Weapon(int weaponID) { activeWeapon = weaponID; }

    public void SetActiveWeapon(int weaponID) { activeWeapon = weaponID; }
    

}

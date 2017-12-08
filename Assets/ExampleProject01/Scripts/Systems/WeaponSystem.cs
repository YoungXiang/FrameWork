using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork;
using Artemis;
using Artemis.System;
using System;

[Artemis.Attributes.ArtemisEntitySystem(
    ExecutionType = Artemis.Manager.ExecutionType.Synchronous,
    GameLoopType = Artemis.Manager.GameLoopType.Update,
    Layer = 1)]
public class WeaponSystem : EntityProcessingSystem
{
    public WeaponSystem() : base(Aspect.All(typeof(Weapon)))
    {
    }

    public override void Process(Entity entity)
    {
        Weapon weapon = entity.GetComponent<Weapon>();
        if (weapon.activeWeapon < 0) return;

        if (!IsCanUseWeapon(weapon)) return;
        
        WeaponConfig weaponConfig = EntityWorldRegistry.Instance.weaponConfigs[weapon.activeWeapon];
        if (weapon.skillInst == null)
        {
            weapon.skillInst = new SkillBase(weaponConfig);
            weapon.skillInst.Use();
        }
        else
        {
            weapon.skillInst.Update();
        }
    }

    public bool IsCanUseWeapon(Weapon weapon)
    {
        // check target is dead or not

        // check weapon is in cd or not
        WeaponConfig weaponConfig = EntityWorldRegistry.Instance.weaponConfigs[weapon.activeWeapon];
        if (weapon.cdCounter >= weaponConfig.cd)
        {
            return true;
        }

        return false;
    }
}

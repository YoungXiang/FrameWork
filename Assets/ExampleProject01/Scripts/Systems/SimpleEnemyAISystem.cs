using System;
using System.Collections.Generic;
using UnityEngine;
using FrameWork;
using Artemis;
using Artemis.System;

[Artemis.Attributes.ArtemisEntitySystem(
    ExecutionType = Artemis.Manager.ExecutionType.Synchronous,
    GameLoopType = Artemis.Manager.GameLoopType.Update,
    Layer = 1)]
public class SimpleEnemyAISystem : EntityProcessingSystem
{
    public SimpleEnemyAISystem() : base(Aspect.All(typeof(SimpleEnemyAIComponent)))
    {
    }
    
    // public class Condition // behaviour tree node.
    // { 
    //    public bool CheckCondition(); public void Apply();
    // }
    // Conditions[] conditions;

    // Or

    // Behaviour Tree
    
    public override void Process(Entity ent)
    {
        // check target
        // check ComponentAttackTarget, check if target is dead or not? if is dead, then find next target.
        // else,
        SimpleEnemyAIComponent seac = ent.GetComponent<SimpleEnemyAIComponent>();
        if (seac.targetId == -1)
        {
            seac.targetId = 0;
        }

        // get skill id ,  get skill range 
        Weapon weapon = ent.GetComponent<Weapon>();
        WeaponConfig wConf = EntityWorldRegistry.Instance.weaponConfigs[weapon.activeWeapon];
        if (wConf != null)
        {
            View view = ent.GetComponent<View>();
            View heroView = Global.gHero.GetComponent<View>();
            Vector3 attackPos = (heroView.transform.position - view.transform.position);
            float mag = attackPos.magnitude;
            attackPos = attackPos.normalized * (mag - wConf.attackRange) + view.transform.position;
        }



    }
}

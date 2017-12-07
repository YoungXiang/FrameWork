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
    }
}

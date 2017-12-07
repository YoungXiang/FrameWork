using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork;
using Artemis;

public class MoveToTargetRoutine : AIRoutine
{
    MovePath path;

    public override void Start(AIBrain brain, AIBehaviour behaviour)
    {
        base.Start(brain, behaviour);

        Entity target = EntityWorldRegistry.Instance.entityWorld.GetEntity(brain.targetID);
        
        path = brain.owner.GetComponent<MovePath>();
        path.FindPath(brain.owner.GetComponent<View>().transform.position, target.GetComponent<View>().transform.position);
        urgentRate = path.path.Count / (1.0f - behaviour.escapeUrgentLimit);
    }

    public override void Execute(AIBrain brain, AIBehaviour behaviour)
    {
        if (path.reached)
        {
            Stop(brain, behaviour);
        }
        else
        {
            behaviour.urgent -= urgentRate;
        }
    }
}

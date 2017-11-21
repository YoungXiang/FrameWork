using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork;
using Artemis;
using Artemis.System;

[Artemis.Attributes.ArtemisEntitySystem(
    ExecutionType = Artemis.Manager.ExecutionType.Synchronous,
    GameLoopType = Artemis.Manager.GameLoopType.Update, 
    Layer = 1)]
public class MoveSystem : EntityProcessingSystem
{
    protected World gridWorld;

    public MoveSystem() : base(Aspect.All(typeof(View), typeof(Velocity), typeof(MovePath)))
    {
    }

    public override void Process(Entity ent)
    {
        MovePath path = ent.GetComponent<MovePath>();
        if (path.dstPos == null || path.reached)
        {
            return;
        }

        Velocity vel = ent.GetComponent<Velocity>();
        View view = ent.GetComponent<View>();


        //view.transform.position += vel.value * Time.deltaTime;
    }
}

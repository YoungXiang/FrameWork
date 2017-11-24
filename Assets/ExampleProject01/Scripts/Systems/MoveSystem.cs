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
    public const float epsilon = 0.1f;

    public MoveSystem() : base(Aspect.All(typeof(View), typeof(Velocity), typeof(MovePath)))
    {
    }

    public override void Process(Entity ent)
    {
        MovePath path = ent.GetComponent<MovePath>();
        if (path.dstPos == null || path.reached)
        {
            // Move finished
            return;
        }

        View view = ent.GetComponent<View>();
        if (path.path.Count > 0)
        {
            Velocity vel = ent.GetComponent<Velocity>();
            Vector3 nextDst = path.path[path.currentIndex].worldPosition;
            if (Vector3.Distance(view.transform.position, nextDst) <= epsilon)
            {
                path.currentIndex++;
            }
            else
            {
                // TODO: moveDir 只需计算一次
                Vector3 moveDir = (nextDst - view.transform.position).normalized;
                view.transform.position += moveDir * vel.value * Time.deltaTime;
            }
        }
        else
        {
            EntityWorldRegistry.Instance.gridWorld.FindPath(view.transform.position, path.dstPos, path.path);
        }
    }
}

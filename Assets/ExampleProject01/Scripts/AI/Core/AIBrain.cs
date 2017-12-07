using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artemis;
using Artemis.System;

namespace FrameWork
{
    /*
    [Artemis.Attributes.ArtemisEntitySystem(
        ExecutionType = Artemis.Manager.ExecutionType.Synchronous,
        GameLoopType = Artemis.Manager.GameLoopType.Update,
        Layer = 1)]
    // AIBrain is like a process, and a behavior is like a thread
    public class AIBrainSystem : EntityProcessingSystem
    {
        public AIBrainSystem() : base(Aspect.All(typeof(AIBrain)))
        {
        }
        
        public override void Process(Entity ent)
        {
            AIBrain brain = ent.GetComponent<AIBrain>();
            brain.Update(ent);
        }
    }
    */

    [Artemis.Attributes.ArtemisComponentPool(
        InitialSize = ComponentDefine.ComponentPoolSize,
        IsResizable = true,
        IsSupportMultiThread = true,
        ResizeSize = ComponentDefine.ResizeScale)]
    public abstract class AIBrain : ComponentPoolable
    {
        #region Datas
        // attack single target
        public int targetID;
        public Entity owner;
        #endregion

        // probably serialized
        public AIBehaviour[] behaviours;
        //protected Heap<AIBehaviour> heap;

        /// <summary>
        /// Current behaviour index
        /// </summary>
        [System.NonSerialized]
        public int cur = -1;

        public virtual void Update(Entity ownerEnt)
        {
            owner = ownerEnt;
            UpdateFactors();
            UpdateBehaviours();
        }

        /// <summary>
        /// Evaluate the environment
        /// </summary>
        protected virtual void UpdateFactors()
        {

        }

        /// <summary>
        /// Decide behaviours
        /// </summary>
        protected virtual void UpdateBehaviours()
        {
            if (behaviours.Length <= 0) return;

            int i = FindNextBehaviour();
            if (i < 0) return;

            if (cur != i)
            {
                if (cur >= 0) behaviours[cur].Exit(this);
                behaviours[i].Enter(this);
                cur = i;
            }
            else
            {
                behaviours[cur].Update(this);
            }
        }

        protected abstract int FindNextBehaviour();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    /*
    public abstract class CustomYieldInstruction : IEnumerator
    {
        public object Current
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        ///   <para>Indicates if coroutine should be kept suspended.</para>
        /// </summary>
        public abstract bool keepWaiting
        {
            get;
        }

        protected CustomYieldInstruction()
        {
        }

        public bool MoveNext()
        {
            return this.keepWaiting;
        }

        public void Reset()
        {
        }
    }
    */

    /// <summary>
    /// A routine is a sub state of a behaviour, but it also contains data.
    /// e.g: A Find target routine should have a target data, that can be shared within the behaviour.
    /// </summary>
    public abstract class AIRoutine
    {
        public bool isFinished { get; set; }

        public float urgentRate = 0.0f;

        public virtual void Start(AIBrain brain, AIBehaviour behaviour)
        {
            isFinished = true;
        }
        public abstract void Execute(AIBrain brain, AIBehaviour behaviour);
        public virtual void Stop(AIBrain brain, AIBehaviour behaviour)
        {
            isFinished = false;
        }
    }
}

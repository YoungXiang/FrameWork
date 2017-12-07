using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artemis;

namespace FrameWork
{
    public class AIBehaviour : IHeapItem<AIBehaviour>
    {
        public AIRoutine[] routines;
        protected int currentRoutine;

        // from 0 - 1;
        public float urgent;
        // can be break when urgent value is lower than
        public float escapeUrgentLimit = 0.8f;
        // is the behavior running?
        public bool isRunning = false;

        int heapIndex;
        public int HeapIndex
        {
            get { return heapIndex; }
            set { heapIndex = value; }
        }

        protected virtual void SetupBehaviour(AIBrain brain)
        {

        }
        
        public virtual bool IsCanInterrupt()
        {
            return urgent < escapeUrgentLimit;
        }

        #region StateMachine Control
        public virtual void Enter(AIBrain brain)
        {
            isRunning = true;
            currentRoutine = 0;
        }

        public virtual void Update(AIBrain brain)
        {
            if (currentRoutine < 0) return;

            if (!routines[currentRoutine].isFinished)
            {
                routines[currentRoutine].Execute(brain, this);
            }
            else
            {
                currentRoutine++;
                if (currentRoutine >= routines.Length)
                {
                    currentRoutine = -1;
                }
                else
                {
                    routines[currentRoutine].Start(brain, this);
                }
            }
        }
        
        public virtual void Exit(AIBrain brain)
        {
            isRunning = false;
        }
        #endregion

        public int CompareTo(AIBehaviour other)
        {
            if (isRunning && (urgent > escapeUrgentLimit))
            {
                return -1;
            }

            return other.urgent - urgent > 0 ? 1 : -1;
        }
    }
}

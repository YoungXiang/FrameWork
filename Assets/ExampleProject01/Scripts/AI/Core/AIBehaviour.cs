using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class AIBehaviour : ScriptableObject, IHeapItem<AIBehaviour>
    {
        public AIRoutine[] routines;
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

        public void AddFactor(string factor, float importance)
        {

        }

        public void UpdateFactors()
        {

        }

        public void Execute(AIBrain brain)
        {

        }

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

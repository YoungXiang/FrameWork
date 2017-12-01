using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FrameWork
{
    // AIBrain is like a process, and a behavior is like a thread
    public abstract class AIBrain : MonoBehaviour
    {
        // probably serialized
        public AIBehaviour[] behaviours;
        //protected Heap<AIBehaviour> heap;

        /// <summary>
        /// Current behaviour index
        /// </summary>
        [System.NonSerialized]
        public int cur = -1;

        protected virtual void Update()
        {
            if (behaviours.Length <= 0) return;

            int i = FindNextBehaviour();
            if (i >= 0)
            {
                behaviours[i].Execute(this);
            }
            /*
            int count = behaviours.Length;
            if (count <= 0) return;

            heap.Clear();

            // update factors
            for (int i = 0; i < count; i++)
            {
                behaviours[i].UpdateFactors();
                heap.Add(behaviours[i]);
            }

            // sort the behaviours using its urgent value
            // if behavior is running
            */
        }

        protected abstract int FindNextBehaviour();
    }
}

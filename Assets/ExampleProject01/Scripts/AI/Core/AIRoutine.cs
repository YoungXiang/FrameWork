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

    public class AIRoutine : CustomYieldInstruction
    {
        public override bool keepWaiting { get { return false; } }
    }
}

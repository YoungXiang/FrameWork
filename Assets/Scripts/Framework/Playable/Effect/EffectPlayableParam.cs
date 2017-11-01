using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class EffectPlayableParam : IPlayableParams
    {

        /// <summary>
        /// Effect prefab
        /// </summary>
        public GameObject effectObj;

        public EffectPlayableParam(GameObject owner_, bool newInstance_, int loopTime_)

            : base(owner_, newInstance_, loopTime_)
        {

        }
    }
}

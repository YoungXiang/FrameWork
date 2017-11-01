using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class AnimationPlayableParam : IPlayableParams
    {
        /// <summary>
        /// if clip is null, then use the path to load it, if not, this is never used.
        /// </summary>
        //public string clipAssetPath;
        public AnimationClip clip;

        public float transitionTime;

        /// <summary>
        /// Translate to first frame when finish.
        /// </summary>
        public bool idleWhenFinish;

        public AnimationPlayableParam(GameObject owner_, bool newInstance_, int loopTime_, 
            string clipAssetPath_, float transitionTime_ = 0.0f, bool idleWhenFinish = false)
            
            : base(owner_, newInstance_, loopTime_)
        {
            // TODO: clip = assetloader.load(clipAssetPath);
            transitionTime = transitionTime_;
            name = clip.name;
        }

        public AnimationPlayableParam(GameObject owner_, bool newInstance_, int loopTime_, 
            AnimationClip clip_, float transitionTime_ = 0.0f, bool idleWhenFinish = false)
            
            : base(owner_, newInstance_, loopTime_)
        {
            clip = clip_;
            transitionTime = transitionTime_;
            name = clip.name;
        }
    }
}

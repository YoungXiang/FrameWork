using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public abstract class BasePlayable<PlayableParamType> : IPlayable 
        where PlayableParamType : IPlayableParams
    {
        // Delegates
        public delegate void OnPlayableFinishedEvent(BasePlayable<PlayableParamType> playable);
        public OnPlayableFinishedEvent OnPlayableFinished;
        
        // Statics
        protected static int gInstanceID = 0;
        // my instance id
        protected int instanceID;
        protected bool isPlaying = false;

        public PlayableParamType param;

        public BasePlayable()
        {
            instanceID = gInstanceID++;
        }

        public int GetInstanceID() { return instanceID; }

        public virtual void SetParam(PlayableParamType param_)
        {
            param = param_;
            // override to set params
        }

        public virtual bool IsPlaying() { return isPlaying; }
        public abstract void Pause(bool pause);
        public abstract void Play();
        public abstract void Stop();
        public abstract void Update(float dt);
    }
}

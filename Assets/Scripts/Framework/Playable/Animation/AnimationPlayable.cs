using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class AnimationPlayable : BasePlayable<AnimationPlayableParam>
    {
        public Animation animation;
        /// <summary>
        /// Duration (lifetime) in seconds.
        /// </summary>
        public float length;
        public float timePlayed = -1;

        public override void SetParam(AnimationPlayableParam param_)
        {
            base.SetParam(param_);

            animation = param.owner.GetOrCreateComponent<Animation>();
            if (animation.GetClip(param.name) == null)
                animation.AddClip(param.clip, param.name);
            
            length = param.clip.length;
            timePlayed = -1;
        }

        public override bool IsPlaying()
        {
            //return param.loopTime == 0 ? (timePlayed >= 0 && timePlayed <= length) : timePlayed >= 0;
            return isPlaying;
        }

        public override void Pause(bool pause)
        {
            if (animation != null) animation[param.name].speed = pause ? 0.0f : 1.0f;
        }

        public override void Play()
        {
            timePlayed = 0.0f;
            isPlaying = true;
            if (param.transitionTime > 0.0f)
            {
                if (animation != null) animation.CrossFade(param.name, param.transitionTime);
            }
            else
            {
                if (animation != null) animation.Play(param.name);
            }
        }

        public override void Stop()
        {
            timePlayed = -1;
            isPlaying = false;
            
            if (animation != null)
            {
                animation.Stop();
                if (param.idleWhenFinish)
                {
                    string lastClipName = animation.clip.name;
                    animation[lastClipName].time = 0.0f;
                    animation.Play();
                    animation.Sample();
                    animation.Stop();
                }
            }
            if (OnPlayableFinished != null) OnPlayableFinished(this);
        }

        public override void Update(float dt)
        {
            // reached end
            if (timePlayed > length)
            {
                // try replay
                if (param.loopTime == 0)
                {
                    Stop();
                }
                else if (param.loopTime > 0)
                {
                    param.loopTime--;
                    timePlayed = 0.0f;
                    if (animation != null) animation.Play(param.name);
                }
                else if (param.loopTime == -1)
                {
                    timePlayed = 0.0f;
                    if (animation != null) animation.Play(param.name);
                }
            }

            timePlayed += dt;
        }
    }
}

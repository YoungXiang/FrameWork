using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class EffectPlayable : BasePlayable<EffectPlayableParam>
    {
        // all possible effect type
        public ParticleSystem psSfx;
        public TrailRenderer trSfx;
        public Animation amSfx;
        
        public override void SetParam(EffectPlayableParam param_)
        {
            base.SetParam(param_);

            psSfx = param.effectObj.GetComponent<ParticleSystem>();
            trSfx = param.effectObj.GetComponent<TrailRenderer>();
            amSfx = param.effectObj.GetComponent<Animation>();
        }
        
        public override void Pause(bool pause)
        {
            if (psSfx != null) if (pause) psSfx.Pause(); else psSfx.Play();
            //if (trSfx != null) trSfx.();
            if (amSfx != null) amSfx[amSfx.clip.name].speed = pause ? 0 : 1;
        }

        public override void Play()
        {
            isPlaying = true;
            if (psSfx != null)
            {
                psSfx.time = 0;
                psSfx.Play();
            }
            if (amSfx != null)
            {

            }
        }

        public override void Stop()
        {
            isPlaying = false;
        }

        public override void Update(float dt)
        {
            if (param.owner != null)
            {
                param.effectObj.transform.position = param.owner.transform.position;
            }
        }
    }
}

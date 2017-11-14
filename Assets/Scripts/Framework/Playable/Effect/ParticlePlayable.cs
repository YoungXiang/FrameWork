using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class ParticlePlayable : BasePlayable<EffectPlayableParam>
    {
        public ParticleSystem sfx;
        
        public override void Pause(bool pause)
        {
            if (sfx != null) if (pause) sfx.Pause(); else sfx.Play();
        }

        public override void Play()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        public override void Update(float dt)
        {
            throw new NotImplementedException();
        }
    }
}

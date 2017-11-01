using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class SoundPlayableParams : IPlayableParams
    {
        public AudioClip clip;

        public float volume;

        public float fadeInTime;
        public float fadeOutTime;

        public SoundPlayableParams(GameObject owner_, bool newInstance_, int loopTime_, 
            string audioPath_, float volume_ = 1.0f, float fadeInTime_ = 0.0f, float fadeOutTime_ = 0.0f)

            : base(owner_, newInstance_, loopTime_)
        {
            // TODO : clip = assetloader.load(audioPath_);
            volume = volume_;
            fadeInTime = fadeInTime_;
            fadeOutTime = fadeOutTime_;
            name = clip.name;
        }

        public SoundPlayableParams(GameObject owner_, bool newInstance_, int loopTime_, 
            AudioClip clip_, float volume_ = 1.0f, float fadeInTime_ = 0.0f, float fadeOutTime_ = 0.0f) 

            : base(owner_, newInstance_, loopTime_)
        {
            clip = clip_;
            volume = volume_;
            fadeInTime = fadeInTime_;
            fadeOutTime = fadeOutTime_;
            name = clip.name;
        }
    }
}

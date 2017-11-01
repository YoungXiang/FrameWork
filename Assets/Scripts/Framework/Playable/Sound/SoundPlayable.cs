using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class SoundPlayable : BasePlayable<SoundPlayableParams>
    {
        public AudioSource audioSource;

        /// <summary>
        /// Duration (lifetime) in seconds.
        /// </summary>
        public float length;
        public float timePlayed = -1f;

        protected float fadeInSpeed = 0.0f;
        protected float fadeOutSpeed = 0.0f;

        public override void SetParam(SoundPlayableParams param_)
        {
            base.SetParam(param_);

            audioSource = param.owner.GetOrCreateComponent<AudioSource>();
            audioSource.clip = param.clip;
            audioSource.loop = false;
            audioSource.playOnAwake = false;

            timePlayed = -1;
            length = param.clip.length;
            audioSource.volume = param.volume;
            if (param.fadeInTime > 0)
            {
                audioSource.volume = 0.0f;
                fadeInSpeed = 1 / param.fadeInTime;
            }
            if (param.fadeOutTime > 0)
                fadeOutSpeed = 1 / param.fadeOutTime;
        }

        public override void Pause(bool pause)
        {
            if (pause) audioSource.Pause();
            else audioSource.UnPause();
        }

        public override void Play()
        {
            timePlayed = 0.0f;
            isPlaying = true;
            audioSource.Play();
        }

        public override void Stop()
        {
            timePlayed = -1;
            isPlaying = false;
            // always stop before callback
            audioSource.Stop();
            if (OnPlayableFinished != null) OnPlayableFinished(this);
        }

        public override void Update(float dt)
        {
            if (param.fadeInTime > 0)
            {
                param.fadeInTime -= dt;
                audioSource.volume = Mathf.Clamp01(audioSource.volume + fadeInSpeed * dt);
            }

            // last loop
            if (param.loopTime == 0)
            {
                if (param.fadeOutTime > 0)
                {
                    if (timePlayed >= length - param.fadeOutTime)
                    {
                        param.fadeOutTime -= dt;
                        audioSource.volume = Mathf.Clamp01(audioSource.volume - fadeInSpeed * dt);
                    }
                }
            }

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
                    audioSource.Play();
                }
                else if (param.loopTime == -1)
                {
                    timePlayed = 0.0f;
                    audioSource.Play();
                }
            }

            timePlayed += dt;
        }
    }
}

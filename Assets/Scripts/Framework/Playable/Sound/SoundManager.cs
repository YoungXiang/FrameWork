using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class SoundManager : PlayableManager<SoundManager, SoundPlayable, SoundPlayableParams>
    {
        private AudioListener audioListener;
        private Transform player;

        #region API
        public void Set3DPlayer(Transform player3d)
        {
            player = player3d;
        }

        public void Stop2DSound(string name)
        {
            GameObject owner;
            Transform ownerTrans = transform.Find(name);
            if (ownerTrans != null)
            {
                owner = ownerTrans.gameObject;
                Stop(owner, name);
            }
        }

        /// <summary>
        /// 2D Sound is special, which needs no owner to pass in.
        /// </summary>
        /// <param name="clip_"></param>
        /// <param name="loopTime_"></param>
        /// <param name="volume_"></param>
        /// <param name="fadeInTime_"></param>
        /// <param name="fadeOutTime_"></param>
        public void Play2DSound(AudioClip clip_, int loopTime_ = 0, float volume_ = 1.0f, float fadeInTime_ = 0.0f, float fadeOutTime_ = 0.0f)
        {
            GameObject owner;
            Transform ownerTrans = transform.Find(clip_.name);
            if (ownerTrans != null)
            {
                owner = ownerTrans.gameObject;
            }
            else
            {
                owner = GameObjectPool.Instance.Create(clip_.name);
                owner.transform.SetParent(transform, false);
            }

            SoundPlayableParams param = new SoundPlayableParams(owner, false, loopTime_, clip_, volume_, fadeInTime_, fadeOutTime_);
            SoundPlayable playable = Play(param);
            playable.OnPlayableFinished += OnSoundPlayFinishedCallback;
        }

        private void OnSoundPlayFinishedCallback(BasePlayable<SoundPlayableParams> playable)
        {
            Debug.Log("OnSoundPlayFinished ");
            GameObjectPool.Instance.Recycle(playable.param.owner);
            playable.OnPlayableFinished -= OnSoundPlayFinishedCallback;
        }
        #endregion
        
        #region Engine Callbacks
        private void Awake()
        {
            audioListener = gameObject.GetOrCreateComponent<AudioListener>();
        }

        private void LateUpdate()
        {
            if (player != null)
            {
                transform.position = player.position;
                transform.rotation = player.rotation;
            }
        }
        #endregion
    }
}

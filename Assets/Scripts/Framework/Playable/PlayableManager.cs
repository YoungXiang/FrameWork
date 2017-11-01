using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class PlayableManager<ClassType, PlayableType, PlayableParamType> : SingleBehaviour<ClassType> 
        where ClassType : Component 
        where PlayableType : BasePlayable<PlayableParamType>, new()
        where PlayableParamType : IPlayableParams
    {
        protected Dictionary<int, PlayableType> playableMap = new Dictionary<int, PlayableType>();
        protected Dictionary<string, int> playableNameMap = new Dictionary<string, int>();
        protected List<int> playing = new List<int>();
        protected List<int> played = new List<int>();

        #region Public API
        // TODO : Put link param with id to avoid gc?. Play(int id), Stop(id)
        public virtual PlayableType Play(int id) { return null; }
        public PlayableType Play(PlayableParamType param)
        {
            // TODO : Use object pool
            PlayableType playable = null;
            if (!playableNameMap.ContainsKey(param.nameForInstance))
            {
                param.newInstance = true;
            }
            
            if (param.newInstance)
            {
                playable = new PlayableType();
                playableMap.Add(playable.GetInstanceID(), playable);
                playing.Add(playable.GetInstanceID());
                playableNameMap.Add(param.nameForInstance, playable.GetInstanceID());
            }
            else
            {
                playable = playableMap[playableNameMap[param.nameForInstance]];
            }

            playable.SetParam(param);
            playable.Play();

            return playable;
        }
        
        public bool IsPlaying(GameObject owner, string name)
        {
            return (playableNameMap.ContainsKey(IPlayableParams.GetNameForInstance(owner, name)));
        }

        public void Stop(GameObject owner, string name)
        {
            if (IsPlaying(owner, name))
            {
                playableMap[playableNameMap[IPlayableParams.GetNameForInstance(owner, name)]].Stop();
            }
        }

        public void Stop()
        {
            if (playing.Count > 0)
            {
                for (int i = 0; i < playing.Count; i++)
                {
                    PlayableType plt = playableMap[playing[i]];
                    plt.Stop();
                }
            }
            playing.Clear();
            playableMap.Clear();
            playableNameMap.Clear();
        }
        #endregion

        #region Engine Callbacks
        protected void Update()
        {
            // update playing list
            if (playing.Count <= 0) return;
            for (int i = 0; i < playing.Count; i++)
            {
                PlayableType plt = playableMap[playing[i]];
                if (plt.IsPlaying())
                {
                    playableMap[playing[i]].Update(Time.deltaTime);
                }
                else
                {
                    played.Add(playing[i]);
                }
            }

            // remove finished
            if (played.Count <= 0) return;
            for (int i = 0; i < played.Count; i++)
            {
                playableNameMap.Remove(playableMap[played[i]].param.nameForInstance);
                playing.Remove(played[i]);
                playableMap.Remove(played[i]);
            }
            played.Clear();
        }
        
        public override void OnDestroy()
        {
            Stop();
            base.OnDestroy();
        }
        #endregion

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class SfxManager : SingleBehaviour<SfxManager>
    {
        //Dictionary<int, GameObject> _sfxList = new Dictionary<int, GameObject>();
        //Dictionary<string, GameObject> _sfxStrList = 

        List<ISfx> playing = new List<ISfx>();
        List<ISfx> gcList = new List<ISfx>();
        
        public void Play(ISfx sfxObj)
        {
            sfxObj.Play();
            playing.Add(sfxObj);
        }
        
        public void Update()
        {
            for (int i = 0; i < playing.Count; i++)
            {
                if (!playing[i].Update(Time.deltaTime))
                {
                    gcList.Add(playing[i]);
                }
            }

            if (gcList.Count > 0)
            {
                for (int i = 0; i < gcList.Count; i++)
                {
                    gcList[i].Stop();
                    playing.Remove(gcList[i]);
                }
                gcList.Clear();
            }
        }

        public void OnGUI()
        {
            for (int i = 0; i < playing.Count; i++)
            {
                playing[i].OnGUI();
            }
        }
    }
}

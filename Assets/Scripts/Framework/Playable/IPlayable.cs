using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public interface IPlayable
    {
        int GetInstanceID();
        void Play();
        void Pause(bool pause);
        void Stop();
        void Update(float dt);
        bool IsPlaying();
    }
}

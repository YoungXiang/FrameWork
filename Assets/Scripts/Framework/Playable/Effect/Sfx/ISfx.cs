using System.Collections;
using System.Collections.Generic;

namespace FrameWork
{
    public interface ISfx
    {
        void Play();
        void Stop();
        // Gets Updated in the scene, if return false, then should stop
        bool Update(float dt);
        // When Scene.OnGUI gets called.
        void OnGUI();
    }

    public delegate void OnSfxStop(ISfx sfx);
}

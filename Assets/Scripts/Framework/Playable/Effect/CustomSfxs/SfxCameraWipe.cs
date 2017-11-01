using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class SfxCameraWipe : ISfx
    {
        public Camera from, to;
        public float time;
        public float lifeTime;
        protected OnSfxStop onSfxStop;
        protected RenderTexture renderTex;
        protected Color guiColor;
        protected Rect guiRect;

        public SfxCameraWipe(Camera fromCam, Camera toCam, float duration, OnSfxStop onSfxStopCallback = null)
        {
            from = fromCam;
            to = toCam;
            time = 0.0f;
            lifeTime = duration;
            onSfxStop = onSfxStopCallback;

            guiRect = new Rect(0, 0, Screen.width, Screen.height);
            renderTex = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.Default);
        }

        public void Play()
        {
            from.targetTexture = renderTex;
            from.Render();
            from.gameObject.SetActive(false);
            from.targetTexture = null;
            to.gameObject.SetActive(true); 
        }

        public void Stop()
        {
            if (onSfxStop != null)
            {
                onSfxStop(this);
            }
        }

        public bool Update(float dt)
        {
            if (time <= lifeTime)
            {
                time += dt;
                return true;
            }

            return false;
        }

        public void OnGUI()
        {
            if (time <= lifeTime)
            {
                GUI.depth = -9999999;
                guiColor = GUI.color;
                guiColor.a = 1 - (time/lifeTime);
                GUI.color = guiColor;
                GUI.DrawTexture(guiRect, renderTex);
            }
        }
    }
}

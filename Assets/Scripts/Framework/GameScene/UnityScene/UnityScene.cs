using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FrameWork
{
    public class UnityScene : BaseScene
    {
        protected AsyncOperation loadOperation;
        public override float progress
        {
            get
            {
                if (loadOperation != null)
                {
                    //if (loadOperation.progress >= 0.9f)
                    //{
                    //    loadOperation.allowSceneActivation = true;
                    //}
                    return loadOperation.progress;
                }

                return 0.0f;
            }
        }
        
        public override void Load()
        {
            loadOperation = AssetManager.LoadScene(path);
            //loadOperation.allowSceneActivation = false;
        }
        
        public override void Unload()
        {
            OnBeforeUnload();
            AssetManager.UnloadScene(path);
        }

        public override void Update()
        {
        }

        public override void OnLoaded()
        {

        }

        public override void OnBeforeUnload()
        {

        }
    }
}
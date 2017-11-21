using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FrameWork
{
    public class UnityScene : BaseScene
    {
        protected float _progress = 0.0f;
        public override float progress
        {
            get
            {
                return _progress;
            }
        }
        
        public override void Load()
        {
            AssetManager.AddListenerForAsset(path, (IAsyncRequestBase request) => 
            {
                SceneAsyncRequest sr = request as SceneAsyncRequest;
                if (sr != null && sr.asyncOp != null)
                {
                    _progress = sr.asyncOp.progress;
                    if (sr.asyncOp.progress >= 0.9f)
                    {
                        _progress = 1.0f;
                    }
                }
            });
            AssetManager.LoadScene(path, true);
        }

        public sealed override void Unload()
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
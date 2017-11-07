using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public abstract class ISceneBuilder
    {
        protected string sceneName;
        protected string scenePath;
        public ISceneBuilder(string sceneName_, string scenePath_) { sceneName = sceneName_; scenePath = scenePath_; }
        public abstract BaseScene Build();
    }

    // Using generic template to build the template.
    public class SceneBuilder<T>  : ISceneBuilder where T : BaseScene, new()
    {
        public SceneBuilder(string sceneName_, string scenePath_) :base(sceneName_, scenePath_) { }

        public override BaseScene Build()
        {
            T s = new T();
            s.name = sceneName;
            s.path = scenePath;
            s.Load();
            return s;
        }
    }

    // this behaves pretty much like the state machine
    // only the scene object has different behaviours than a state.
    public class SceneManager : SingleBehaviour<SceneManager>
    {
        /// <summary>
        /// Note that only public field is serialized by default.
        /// </summary>
        protected BaseScene currentRunningScene = null;
        protected BaseScene currentLoadingScene = null;

        protected BaseSceneTransition currentTransition = null;

        protected Dictionary<string, ISceneBuilder> sceneBuilder = new Dictionary<string, ISceneBuilder>();

        public void RegisterScene<T>(string sceneName, string scenePath) where T : BaseScene, new()
        {
            if (!sceneBuilder.ContainsKey(sceneName))
            {
                sceneBuilder.Add(sceneName, new SceneBuilder<T>(sceneName, scenePath));
            }
        }

        public void LoadScene(string sceneName, BaseSceneTransition transition = null)
        {
            if (currentLoadingScene != null)
            {
                Debug.LogWarning("Current loading task is not finished : " + currentLoadingScene.progress);
                return;
            }

            if (!sceneBuilder.ContainsKey(sceneName))
            {
                Debug.LogErrorFormat("Scene doesn't have a builder. Do you forget to register first ? [{0}].", sceneName);
                return;
            }

            currentLoadingScene = sceneBuilder[sceneName].Build();
            currentTransition = transition;

            if (currentRunningScene != null)
            {
                currentRunningScene.Unload();
            }
        }

        private void Update()
        {
            if (currentLoadingScene != null)
            {
                currentLoadingScene.Update();
                float progress = currentLoadingScene.progress;
                if (currentTransition != null)
                {
                    currentTransition.UpdateTransition(progress);
                }
                
                if (progress >= 1)
                {
                    currentLoadingScene.OnLoaded();
                    currentRunningScene = currentLoadingScene;
                    currentLoadingScene = null;
                }
            }
            else if (currentRunningScene != null)
            {
                currentRunningScene.Update();
            }
        }
    }
}

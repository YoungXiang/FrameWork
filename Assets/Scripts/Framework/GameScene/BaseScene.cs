using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public abstract class BaseScene
    {
        /// <summary>
        /// Progress of loading the scene
        /// </summary>
        public virtual float progress { get { return 0.0f; } }

        public string name;
        // scene asset path
        public string path;

        public BaseScene() { }

        public BaseScene(string name_, string path_)
        {
            name = name_;
            path = path_;
        }

        public abstract void Load();
        public abstract void Unload();

        public abstract void Update();

        /// <summary>
        /// Used for Scene Initialization
        /// </summary>
        public abstract void OnLoaded();

        /// <summary>
        /// Used for Scene Clear
        /// </summary>
        public abstract void OnBeforeUnload();
    }
}

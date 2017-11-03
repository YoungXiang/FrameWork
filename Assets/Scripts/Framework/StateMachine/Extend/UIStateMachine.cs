using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public abstract class UIStateMachine : MonoStateMachine
    {
        [Tooltip("Which canvas should holds this UI")]
        public int layer;
        [Tooltip("Sorting order inside the canvas")]
        public int sortingOrder = 0;
        [Tooltip("Will this UI always bring to top when shown")]
        public bool bringToFirst = false;
        [Tooltip("Should unload when unity scene unloaded")]
        public bool unloadWhenSceneUnload = true;
    }
}

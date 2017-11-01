using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    // this is only a player when transitioning scene
    public abstract class BaseSceneTransition : MonoBehaviour
    {
        public abstract void UpdateTransition(float progress);
    }
}
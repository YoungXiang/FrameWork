using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class KeyCodeManager : Singleton<KeyCodeManager>
    {
        public bool enabled = true;

        private Dictionary<KeyCode, Stack<System.Action>> keyCodeEventsSingle_ = new Dictionary<KeyCode, Stack<System.Action>>();

        /*
        public void RegisterKeyCodeEvent(KeyCode keyCode, System.Action callback)
        {

        }

        public void UnregisterKeyCodeEvent(KeyCode keyCode, System.Action callback)
        {

        }
        */

        public void PushKeyCodeEvent(KeyCode keyCode, System.Action callback)
        {
            if (!keyCodeEventsSingle_.ContainsKey(keyCode))
            {
                keyCodeEventsSingle_[keyCode] = new Stack<System.Action>();
            }

            keyCodeEventsSingle_[keyCode].Push(callback);
        }

        public void PopKeyCodeEvent(KeyCode keyCode)
        {
            if (keyCodeEventsSingle_.ContainsKey(keyCode))
            {
                keyCodeEventsSingle_[keyCode].Pop();
            }
        }

        public void Update()
        {
            if (!enabled) return;

            if (Input.anyKeyDown)
            {
                foreach (KeyValuePair<KeyCode, Stack<System.Action>> pair in keyCodeEventsSingle_)
                {
                    if (Input.GetKeyDown(pair.Key))
                    {
                        System.Action callback = pair.Value.Peek();
                        callback.Invoke();
                    }
                }
            }
        }
    }
}

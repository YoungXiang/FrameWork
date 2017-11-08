using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class EntityData : DataListener
    {
        // Absolutely Unique ID.
        public int pid;
        public override void DispatchOnValueChanged<T>(string key, T value)
        {
            EventDispatcher.DispatchEvent(string.Format("{0}_{1}", pid, key), value);
        }
        public new string name = "EntityData";
    }
}

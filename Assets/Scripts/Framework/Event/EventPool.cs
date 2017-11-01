using System.Collections.Generic;

namespace FrameWork
{
    internal class EventPool : Singleton<EventPool>
    {
        private Dictionary<string, EventAction> listeners;

        public override void Init()
        {
            listeners = new Dictionary<string, EventAction>();
        }

        public void RegisterListener(string eventName, EventAction action)
        {
            if (!listeners.ContainsKey(eventName))
            {
                // Create a copy 
                listeners.Add(eventName, new EventAction().Join(action));
            }
            else
            {
                // Perform Join Action
                listeners[eventName].Join(action);
            }
        }

        public void UnRegisterListener(string eventName, EventAction action)
        {
            if (listeners.ContainsKey(eventName))
            {
                listeners[eventName].Leave(action);
            }
        }

        public EventAction Get(string eventName)
        {
            if (!listeners.ContainsKey(eventName))
            {
                UnityEngine.Debug.LogWarningFormat("[Event]:Event[eventName = {0}] is not registered!", eventName);
                return null;
            }

            return listeners[eventName];
        }

        public void Clear(string eventName)
        {
            if (listeners.ContainsKey(eventName))
            {
                listeners.Remove(eventName);
            }
        }

        public void ClearAll()
        {
            listeners.Clear();
        }
    }
}

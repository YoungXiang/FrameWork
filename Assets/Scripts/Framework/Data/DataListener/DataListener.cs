using System;
using System.Collections.Generic;

namespace FrameWork
{
    public class DataListener
    {
        protected Dictionary<string, object> collection = new Dictionary<string, object>();
        protected Dictionary<string, List<Delegate>> listeners = new Dictionary<string, List<Delegate>>();

        public string name;
        public DataListener()
        {

        }
        public DataListener(string name_)
        {
            name = name_;
        }

        #region Data 
        public void RegisterAttr<T>(string key, T val)
        {
            if (!collection.ContainsKey(key))
            {
                collection.Add(key, val);
            }
        }

        public object this[string key]
        {
            get
            {
                if (collection.ContainsKey(key)) return collection[key];
                return null;
            }
            set
            {
                if (collection.ContainsKey(key))
                {
                    if (!collection[key].Equals(value))
                    {
                        collection[key] = value;
                        // notify on value changed
                        if (listeners.ContainsKey(key))
                        {
                            if (listeners[key] != null)
                            {
                                var callbacks = listeners[key];
                                for (int i = 0; i < callbacks.Count; i++)
                                {
                                    Action<object> callback = callbacks[i] as Action<object>;
                                    if (callback == null)
                                    {
                                        throw new Exception(string.Format("Invoke {0} error: types of parameters are not match.", key));
                                    }
                                    try
                                    {
                                        callback(value);
                                    }
                                    catch (Exception ex)
                                    {
                                        UnityEngine.Debug.LogError(ex);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
        
        #region Listener
        public void RegisterEvent<T>(string key, Action<T> evt)
        {
            if (!listeners.ContainsKey(key))
            {
                listeners.Add(key, new List<Delegate>());
            }
            listeners[key].Add(evt);
        }
        
        public void UnRegisterEvent<T>(string key, Action<T> evt)
        {
            if (listeners.ContainsKey(key))
            {
                listeners[key].Remove(evt);
                //Delegate.Remove(listeners[key], evt);
            }
        }

        public void ClearEvent(string key)
        {
            if (listeners.ContainsKey(key))
            {
                listeners[key].Clear();
                listeners[key] = null;
                listeners.Remove(key);
            }
        }
        #endregion
    }
}

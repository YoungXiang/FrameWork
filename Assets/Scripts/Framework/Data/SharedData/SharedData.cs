using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class SharedData : Singleton<SharedData>
    {
        // provide easy access to named data
        private Dictionary<string, DataListener> mappedData;
        // global data
        private DataListener globalData;

        public override void Init()
        {
            mappedData = new Dictionary<string, DataListener>();
            globalData = new DataListener("Global");
        }
        
        #region Global Data
        public object this[string key]
        {
            get
            {
                return globalData[key];
            }
            set
            {
                globalData[key] = value;
            }
        }

        public void RegisterGlobalData<T>(string key, T val) 
        {
            globalData.RegisterAttr(key, val);
        }

        public void AddGlobalListener<T>(string key, Action<T> onValueChanged)
        {
            globalData.RegisterEvent<T>(key, onValueChanged);
            //EventListener.Do<T>(onValueChanged).When(string.Format("{0}_{1}", "Global", key));
        }

        public void RemoveGlobalListener<T>(string key, Action<T> onValueChanged)
        {
            globalData.UnRegisterEvent<T>(key, onValueChanged);
            //EventListener.Undo<T>(string.Format("{0}_{1}", "Global", key), onValueChanged);
        }
        #endregion

        #region Independent Data
        public DataListener NewData(string name)
        {
            if (!mappedData.ContainsKey(name))
            {
                mappedData.Add(name, new DataListener());
            }

            return mappedData[name];
        }

        public DataListener GetData(string name)
        {
            if (mappedData.ContainsKey(name)) return mappedData[name];

            return null;
        }

        public void DeleteData(string name)
        {
            if (mappedData.ContainsKey(name))
            {
                mappedData.Remove(name);
            }
        }

        public void AddListener<T>(string name, string key, Action<T> onValueChanged)
        {
            //EventListener.Do<T>(onValueChanged).When(string.Format("{0}_{1}", id, key));
            if (mappedData.ContainsKey(name))
            {
                mappedData[name].RegisterEvent(key, onValueChanged);
            }
        }

        public void RemoveListener<T>(string name, string key, Action<T> onValueChanged)
        {
            //EventListener.Undo<T>(string.Format("{0}_{1}", id, key), onValueChanged);
            if (mappedData.ContainsKey(name))
            {
                mappedData[name].UnRegisterEvent(key, onValueChanged);
            }
        }
        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class SharedData : Singleton<SharedData>
    {
        private Type typeofInt = typeof(int);
        private Type typeofFloat = typeof(float);
        private Type typeofBool = typeof(bool);
        private Type typeofString = typeof(string);

        // data that associate with and id.
        private Dictionary<int, DataListener> mappedData;
        // global data
        private DataListener globalData;

        public override void Init()
        {
            globalData = new DataListener("Global");
        }

        public int this[string key, int defaultVal]
        {
            get
            {
                return globalData[key, defaultVal];
            }
            set
            {
                globalData[key, defaultVal] = value;
            }
        }

        public float this[string key, float defaultVal]
        {
            get
            {
                return globalData[key, defaultVal];
            }
            set
            {
                globalData[key, defaultVal] = value;
            }
        }

        public bool this[string key, bool defaultVal]
        {
            get
            {
                return globalData[key, defaultVal];
            }
            set
            {
                globalData[key, defaultVal] = value;
            }
        }

        public string this[string key, string defaultVal]
        {
            get
            {
                return globalData[key, defaultVal];
            }
            set
            {
                globalData[key, defaultVal] = value;
            }
        }

        public void RegisterGlobalData<T>(string key, T val) where T:struct
        {
            Type typeofT = typeof(T);
            if (typeofT == typeofInt)
            {
                globalData.RegisterAttr(key, Convert.ToInt32(val));
            }
            else if (typeofT == typeofFloat)
            {
                globalData.RegisterAttr(key, Convert.ToSingle(val));
            }
            else if (typeofT == typeofBool)
            {
                globalData.RegisterAttr(key, Convert.ToBoolean(val));
            }
            else if (typeofT == typeofString)
            {
                globalData.RegisterAttr(key, Convert.ToString(val));
            }
            else
            {
                Debug.LogErrorFormat("[DataListener] {0} type not supported.", typeofT.ToString());
            }
        }

        public void AddGlobalListener<T>(string key, Action<T> onValueChanged)
        {
            EventListener.Do<T>(onValueChanged).When(string.Format("{0}_{1}", "Global", key));
        }

        public void RemoveGlobalListener<T>(string key, Action<T> onValueChanged)
        {
            EventListener.Undo<T>(string.Format("{0}_{1}", "Global", key), onValueChanged);
        }

        public void AddListener<T>(int id, string key, Action<T> onValueChanged)
        {
            EventListener.Do<T>(onValueChanged).When(string.Format("{0}_{1}", id, key));
        }

        public void RemoveListener<T>(int id, string key, Action<T> onValueChanged)
        {
            EventListener.Undo<T>(string.Format("{0}_{1}", id, key), onValueChanged);
        }
    }
}

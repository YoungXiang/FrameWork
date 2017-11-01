using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameWork
{
    public class EventDispatcher
    {
        #region Dispatch
        public static void DispatchEvent(string name)
        {
            EventAction eAct = EventPool.Instance.Get(name);
            if (eAct != null) eAct.Invoke();
        }

        public static void DispatchEvent<T>(string name, T t)
        {
            EventAction eAct = EventPool.Instance.Get(name);
            if (eAct != null) eAct.Invoke(t);
        }

        public static void DispatchEvent<T, V>(string name, T t, V v)
        {
            EventAction eAct = EventPool.Instance.Get(name);
            if (eAct != null) eAct.Invoke(t, v);
        }

        public static void DispatchEvent<T, V, U>(string name, T t, V v, U u)
        {
            EventAction eAct = EventPool.Instance.Get(name);
            if (eAct != null) eAct.Invoke(t, v, u);
        }

        public static void DispatchEvent<T, V, U, W>(string name, T t, V v, U u, W w)
        {
            EventAction eAct = EventPool.Instance.Get(name);
            if (eAct != null) eAct.Invoke(t, v, u, w);
        }
        #endregion
    }
}

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class UnityObjectPool<T> where T : UnityEngine.Object
    {
        public delegate T NewInstance();
        public delegate void DeleteInstance(T instance);

        public NewInstance createDelegate;
        public DeleteInstance deleteDelegate;
        
        protected Dictionary<int, T> used;
        protected Dictionary<int, T> unused;
        
        public int capacity
        {
            get
            {
                return unused.Count + used.Count;
            }
        }

        public UnityObjectPool()
        {
            unused = new Dictionary<int, T>();
            used = new Dictionary<int, T>();
        }

        public UnityObjectPool(int poolSize)
        {
            unused = new Dictionary<int, T>(poolSize);
            used = new Dictionary<int, T>(poolSize);
        }

        public T Create()
        {
            if (createDelegate == null)
            {
                Debug.LogErrorFormat("[UnityObjectPool] - No valid createDelegate assigned.");
                return null;
            }

            if (unused.Count <= 0)
            {
                T o = createDelegate();
                used.Add(o.GetInstanceID(), o);
                return o;
            }

            T obj = unused.First().Value;
            int id = obj.GetInstanceID();
            used.Add(id, obj);
            // mark as used
            unused.Remove(id);

            return obj;
        }

        public void Recycle(T obj)
        {
            int id = obj.GetInstanceID();
            used.Remove(id);
            unused.Add(id, obj);
        }

        public void Destroy()
        {
            if (deleteDelegate == null)
            {
                Debug.LogErrorFormat("[UnityObjectPool] : No valid deleteDelegate assigned.");
                used.Clear(); unused.Clear();
                return;
            }

            T[] objs = GetActives();
            if (objs != null)
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    deleteDelegate(objs[i]);
                }
            }

            used.Clear();
            unused.Clear();
        }

        public T[] GetActives()
        {
            if (used.Count <= 0) return null;
            T[] activeList = used.Values.ToArray();
            return activeList;
        }
    }
}

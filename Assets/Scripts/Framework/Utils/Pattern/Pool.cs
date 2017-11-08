using System;
using System.Collections.Generic;
using System.Linq;

namespace FrameWork
{
    public class Pool<T> where T:new()
    {
        Dictionary<T, bool> used;
        Dictionary<T, bool> unused;
        
        public int capacity
        {
            get { return used.Count + unused.Count; }
        }

        public Pool()
        {
            unused = new Dictionary<T, bool>();
            used = new Dictionary<T, bool>();
        }

        public Pool(int poolSize)
        {
            Init(poolSize);
        }
        
        public void Init(int poolSize, bool aggressive = false)
        {
            unused = new Dictionary<T, bool>(poolSize);
            used = new Dictionary<T, bool>(poolSize);

            if (aggressive)
                for (int i = 0; i < poolSize; i++)
                {
                    unused.Add(NewInstance(), true);
                }
        }

        public virtual T NewInstance()
        {
            return new T();
        }

        public virtual void ReleaseInstance(T obj)
        {

        }

        public void Destroy()
        {
            T[] objs = GetActives();
            if (objs != null)
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    ReleaseInstance(objs[i]);
                }
            }

            used.Clear();
            unused.Clear();
        }

        // return all active instance
        public T[] GetActives()
        {
            if (used.Count <= 0) return null;
            T[] activeList = used.Keys.ToArray();
            return activeList;
        }
        
        // Create From Pool;
        public T Create()
        {
            if (unused.Count <= 0)
            {
                unused.Add(NewInstance(), true);
            }
            
            T obj = unused.First().Key;
            used.Add(obj, true);
            // mark as used
            unused.Remove(obj);
            
            return obj;
        }

        public void Recycle(T obj)
        {
            used.Remove(obj);
            unused.Add(obj, true);
        }
    }
}

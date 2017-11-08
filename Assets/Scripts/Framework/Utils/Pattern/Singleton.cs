using UnityEngine;

public class Singleton<T> where T : new()
{
    protected Singleton() { System.Diagnostics.Debug.Assert(null == instance); Init(); }
    protected static T instance = new T();
    public static T Instance
    {
        get { return instance; }
    }

    public virtual void Init() { }

    public virtual void Destroy() { }
}

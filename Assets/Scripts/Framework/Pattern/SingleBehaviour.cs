using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBehaviour<T> : MonoBehaviour where T: Component
{
    static T instance;
    // To avoid the error: 
    static bool instanceDestroyed = false;
    public static T Instance
    {
        get
        {
            if (instance == null && !instanceDestroyed)
            {
                CreateInstance();
            }
            return instance;
        }
    }

    static void CreateInstance()
    {
        GameObject go = new GameObject(typeof(T).ToString());
        instance = go.AddComponent<T>();
        DontDestroyOnLoad(go);
    }

    public static void DestroyInstance()
    {
        Destroy(instance);
    }

    public virtual void OnInstanceCreated()
    {

    }

    public virtual void OnDestroy()
    {
        instanceDestroyed = true;
        instance = null;
    }
}

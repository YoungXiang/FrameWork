using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Coroutiner : MonoBehaviour
{
    void OnDestroy()
    {
        StopAllCoroutines();
    }
    
    #region statics
    static Coroutiner ct;
    static Coroutiner CreateCoroutiner(string name)
    {
        GameObject go = new GameObject(name);
        DontDestroyOnLoad(go);
        return go.AddComponent<Coroutiner>();
    }
    
    public static void Create()
    {
        ct = CreateCoroutiner("DelayCall");
    }

    public static void Destroy()
    {
        Destroy(ct.gameObject);
    }
    #endregion
}

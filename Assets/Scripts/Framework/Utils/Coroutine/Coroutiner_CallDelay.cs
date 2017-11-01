using System;
using System.Collections;
using UnityEngine;

public partial class Coroutiner : MonoBehaviour
{
    #region Delay Call internal
    IEnumerator _CallDelay(float delayTime, Action callee)
    {
        callee();
        yield return new WaitForSeconds(delayTime);
    }

    IEnumerator _CallDelay<T>(float delayTime, Action<T> callee, T t)
    {
        callee(t);
        yield return new WaitForSeconds(delayTime);
    }

    IEnumerator _CallDelay<T, U>(float delayTime, Action<T, U> callee, T t, U u)
    {
        callee(t, u);
        yield return new WaitForSeconds(delayTime);
    }

    IEnumerator _CallDelay<T, U, V>(float delayTime, Action<T, U, V> callee, T t, U u, V v)
    {
        callee(t, u, v);
        yield return new WaitForSeconds(delayTime);
    }
    #endregion

    public static void CallDelay(float delayTime, Action callee)
    {
        ct.StartCoroutine(ct._CallDelay(delayTime, callee));
    }

    public static void CallDelay<T>(float delayTime, Action<T> callee, T t)
    {
        ct.StartCoroutine(ct._CallDelay(delayTime, callee, t));
    }

    public static void CallDelay<T, U>(float delayTime, Action<T, U> callee, T t, U u)
    {
        ct.StartCoroutine(ct._CallDelay(delayTime, callee, t, u));
    }

    public static void CallDelay<T, U, V>(float delayTime, Action<T, U, V> callee, T t, U u, V v)
    {
        ct.StartCoroutine(ct._CallDelay(delayTime, callee, t, u, v));
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Coroutiner : MonoBehaviour
{
    #region Delay Call internal
    IEnumerator _DelayCall(float delayTime, Action callee)
    {
        yield return new WaitForSeconds(delayTime);
        callee();
    }

    IEnumerator _DelayCall<T>(float delayTime, Action<T> callee, T t)
    {
        yield return new WaitForSeconds(delayTime);
        callee(t);
    }

    IEnumerator _DelayCall<T, U>(float delayTime, Action<T, U> callee, T t, U u)
    {
        yield return new WaitForSeconds(delayTime);
        callee(t, u);
    }

    IEnumerator _DelayCall<T, U, V>(float delayTime, Action<T, U, V> callee, T t, U u, V v)
    {
        yield return new WaitForSeconds(delayTime);
        callee(t, u, v);
    }

    IEnumerator _DelayCall(int delayFrame, Action callee)
    {
        while(delayFrame > 0)
        {
            yield return new WaitForEndOfFrame();
            delayFrame--;
        }
        callee();
    }

    IEnumerator _DelayCall<T>(int delayFrame, Action<T> callee, T t)
    {
        while (delayFrame > 0)
        {
            yield return new WaitForEndOfFrame();
            delayFrame--;
        }
        callee(t);
    }

    IEnumerator _DelayCall<T, U>(int delayFrame, Action<T, U> callee, T t, U u)
    {
        while (delayFrame > 0)
        {
            yield return new WaitForEndOfFrame();
            delayFrame--;
        }
        callee(t, u);
    }

    IEnumerator _DelayCall<T, U, V>(int delayFrame, Action<T, U, V> callee, T t, U u, V v)
    {
        while (delayFrame > 0)
        {
            yield return new WaitForEndOfFrame();
            delayFrame--;
        }
        callee(t, u, v);
    }
    #endregion

    public static void DelayCall(float delayTime, Action callee)
    {
        ct.StartCoroutine(ct._DelayCall(delayTime, callee));
    }

    public static void DelayCall<T>(float delayTime, Action<T> callee, T t)
    {
        ct.StartCoroutine(ct._DelayCall(delayTime, callee, t));
    }

    public static void DelayCall<T, U>(float delayTime, Action<T, U> callee, T t, U u)
    {
        ct.StartCoroutine(ct._DelayCall(delayTime, callee, t, u));
    }

    public static void DelayCall<T, U, V>(float delayTime, Action<T, U, V> callee, T t, U u, V v)
    {
        ct.StartCoroutine(ct._DelayCall(delayTime, callee, t, u, v));
    }

    public static void DelayCall(int delayFrame, Action callee)
    {
        ct.StartCoroutine(ct._DelayCall(delayFrame, callee));
    }

    public static void DelayCall<T>(int delayFrame, Action<T> callee, T t)
    {
        ct.StartCoroutine(ct._DelayCall(delayFrame, callee, t));
    }

    public static void DelayCall<T, U>(int delayFrame, Action<T, U> callee, T t, U u)
    {
        ct.StartCoroutine(ct._DelayCall(delayFrame, callee, t, u));
    }

    public static void DelayCall<T, U, V>(int delayFrame, Action<T, U, V> callee, T t, U u, V v)
    {
        ct.StartCoroutine(ct._DelayCall(delayFrame, callee, t, u, v));
    }
}

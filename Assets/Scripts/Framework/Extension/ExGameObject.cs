using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

namespace FrameWork
{
    public static class ExGameObject
    {
        public static T CopyComponent<T>(this Component comp, T other) where T : Component
        {
            Type type = comp.GetType();
            if (type != other.GetType()) return null; // type mis-match
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] pinfos = type.GetProperties(flags);
            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }

            FieldInfo[] finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }

            return comp as T;
        }
        
        public static T GetOrCreateComponent<T>(this GameObject go) where T : Component
        {
            T com = go.GetComponent<T>();
            if (com == null)
            {
                com = go.AddComponent<T>();
            }

            return com;
        }

        public static GameObject FindOrCreateGameObject(this GameObject parent, string name, params System.Type[] type)
        {
            GameObject go = null;
            Transform trans = parent.transform.Find(name);
            if (trans != null)
            {
                go = trans.gameObject;
            }

            if (null == go)
            {
                go = new GameObject(name, type);
                go.transform.SetParent(parent.transform, false);
            }

            return go;
        }

        /// <summary>
        /// this is specially useful for UI.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uiParent"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T FindChildComponent<T>(this GameObject uiParent, string path) where T : Component
        {
            if (uiParent == null)
            {
                return null;
            }

            Transform childObject = uiParent.transform.Find(path);
            T uiObject = childObject.GetComponent<T>();

            return uiObject;
        }

        public static T FindChildComponent<T>(this MonoBehaviour uiParent, string path) where T : Component
        {
            if (uiParent == null)
            {
                return null;
            }

            Transform childObject = uiParent.transform.Find(path);
            T uiObject = childObject.GetComponent<T>();

            return uiObject;
        }

        // try find component in self first, if not exit then continue to search in child
        public static T GetComponentEx<T>(this GameObject go) where T : Component
        {
            T outCom = go.GetComponent<T>();
            if (outCom == null && go.transform.childCount > 0)
            {
                outCom = go.GetComponentInChildren<T>();
            }

            return outCom;
        }

        public static GameObject FindOrCreateGameObject(string name)
        {
            GameObject go = GameObject.Find(name);
            if (null == go)
            {
                go = new GameObject(name);
            }

            return go;
        }

        /*
        public static string GetUserData(this GameObject go, string key)
        {
            GameObjectUserData userData = go.GetOrCreateComponent<GameObjectUserData>();

            return userData.GetUserData(key);
        }

        public static void SetUserData(this GameObject go, string key, string value)
        {
            GameObjectUserData userData = go.GetOrCreateComponent<GameObjectUserData>();
            userData.SetUserData(key, value);
        }
        */

        public delegate void ChildHandler(GameObject child);

        /// <summary>
        /// Iterates all children of a game object
        /// </summary>
        /// <param name="gameObject">A root game object</param>
        /// <param name="childHandler">A function to execute on each child</param>
        /// <param name="recursive">Do it on children? (in depth)</param>
        public static void IterateChildren(GameObject gameObject, ChildHandler childHandler, bool recursive)
        {
            DoIterate(gameObject, childHandler, recursive);
        }

        /// <summary>
        /// NOTE: Recursive!!!
        /// </summary>
        /// <param name="gameObject">Game object to iterate</param>
        /// <param name="childHandler">A handler function on node</param>
        /// <param name="recursive">Do it on children?</param>
        private static void DoIterate(GameObject gameObject, ChildHandler childHandler, bool recursive)
        {
            foreach (Transform child in gameObject.transform)
            {
                childHandler(child.gameObject);
                if (recursive)
                    DoIterate(child.gameObject, childHandler, true);
            }
        }

        public static void SetLayerRecursively(this GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        public delegate void VoidDelegate();
        /// <summary>
        /// Delay call a function using coroutine.
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="task"></param>
        /// <param name="delay">time delay in seconds</param>
        public static void DelayCall(this MonoBehaviour mono, VoidDelegate task, float delay)
        {
            mono.StartCoroutine(_DelayCall(task, delay));
        }
        
        private static IEnumerator _DelayCall(VoidDelegate task, float time)
        {
            yield return new WaitForSeconds(time);
            if (task != null) task();
        }
    }
}
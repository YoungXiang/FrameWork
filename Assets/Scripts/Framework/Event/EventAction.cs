using System;

namespace FrameWork
{
    public class EventAction
    {
        public string eventName;
        internal Delegate callee;

        #region Init
        // Constructors
        public EventAction() { }
        public EventAction Set(Action action) { callee = action; return this; }
        public EventAction Set<T>(Action<T> action) { callee = action; return this; }
        public EventAction Set<T, U>(Action<T, U> action) { callee = action; return this; }
        public EventAction Set<T, U, V>(Action<T, U, V> action) { callee = action; return this; }
        public EventAction Set<T, U, V, W>(Action<T, U, V, W> action) { callee = action; return this; }
        #endregion

        #region Join & Leave - Cannot avoid GC, but saves a little bit of memory
        // Join the arg to this .
        internal EventAction Join(EventAction target)
        {
            eventName = target.eventName;
            callee = Delegate.Combine(callee, target.callee);
            return this;
        }

        internal EventAction Leave(EventAction target)
        {
            callee = Delegate.Remove(callee, target.callee);
            return this;
        }
        #endregion

        #region When (Register)
        public EventAction When(string eName)
        {
            eventName = eName;
            EventPool.Instance.RegisterListener(eventName, this);
            return this;
        }

        #region UnityEvent Trigger
        public EventAction When(UnityEngine.Events.UnityEvent unityEvent, bool clearBeforeAdd = true)
        {
            if (clearBeforeAdd) unityEvent.RemoveAllListeners();
            unityEvent.AddListener(() => { Invoke(); });
            return this;
        }

        public EventAction When<T>(UnityEngine.Events.UnityEvent<T> unityEvent, bool clearBeforeAdd = true)
        {
            if (clearBeforeAdd) unityEvent.RemoveAllListeners();
            unityEvent.AddListener((T t) => { Invoke(t); });
            return this;
        }

        public EventAction When<T, V>(UnityEngine.Events.UnityEvent<T, V> unityEvent, bool clearBeforeAdd = true)
        {
            if (clearBeforeAdd) unityEvent.RemoveAllListeners();
            unityEvent.AddListener((T t, V v) => { Invoke(t, v); });
            return this;
        }

        public EventAction When<T, V, U>(UnityEngine.Events.UnityEvent<T, V, U> unityEvent, bool clearBeforeAdd = true)
        {
            if (clearBeforeAdd) unityEvent.RemoveAllListeners();
            unityEvent.AddListener((T t, V v, U u) => { Invoke(t, v, u); });
            return this;
        }

        public EventAction When<T, V, U, W>(UnityEngine.Events.UnityEvent<T, V, U, W> unityEvent, bool clearBeforeAdd = true)
        {
            if (clearBeforeAdd) unityEvent.RemoveAllListeners();
            unityEvent.AddListener((T t, V v, U u, W w) => { Invoke(t, v, u, w); });
            return this;
        }
        #endregion

        #endregion

        #region Always
        public EventAction Always()
        {
            // mark as permanent
            return this;
        }
        #endregion

        #region Invoke
        public void Invoke()
        {
            var callbacks = callee.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                Action callback = callbacks[i] as Action;
                if (callback == null)
                {
                    throw new Exception(string.Format("Invoke {0} error: types of parameters are not match.", eventName));
                }
                try
                {
                    callback();
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(ex);
                }
            }
        }

        public void Invoke<T>(T t)
        {
            var callbacks = callee.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                Action<T> callback = callbacks[i] as Action<T>;
                if (callback == null)
                {
                    throw new Exception(string.Format("Invoke {0} error: types of parameters are not match.", eventName));
                }
                try
                {
                    callback(t);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(ex);
                }
            }
        }

        public void Invoke<T, U>(T t, U u)
        {
            var callbacks = callee.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                Action<T, U> callback = callbacks[i] as Action<T, U>;
                if (callback == null)
                {
                    throw new Exception(string.Format("Invoke {0} error: types of parameters are not match.", eventName));
                }
                try
                {
                    callback(t, u);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(ex);
                }
            }
        }

        public void Invoke<T, U, V>(T t, U u, V v)
        {
            var callbacks = callee.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                Action<T, U, V> callback = callbacks[i] as Action<T, U, V>;
                if (callback == null)
                {
                    throw new Exception(string.Format("Invoke {0} error: types of parameters are not match.", eventName));
                }
                try
                {
                    callback(t, u, v);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(ex);
                }
            }
        }

        public void Invoke<T, U, V, W>(T t, U u, V v, W w)
        {
            var callbacks = callee.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                Action<T, U, V, W> callback = callbacks[i] as Action<T, U, V, W>;
                if (callback == null)
                {
                    throw new Exception(string.Format("Invoke {0} error: types of parameters are not match.", eventName));
                }
                try
                {
                    callback(t, u, v, w);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(ex);
                }
            }
        }
        #endregion

        #region Undo - Clear (Unregister)
        /// <summary>
        /// Clears this EventAction
        /// </summary>
        public void Undo()
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                EventPool.Instance.Get(eventName).Leave(this);
            }
        }
        
        internal void Unset(Action action) { callee = (Action)callee - action; if (callee == null || callee.GetInvocationList().Length <= 0) EventPool.Instance.Clear(eventName); }
        internal void Unset<T>(Action<T> action) { callee = (Action<T>)callee - action; if (callee == null || callee.GetInvocationList().Length <= 0) EventPool.Instance.Clear(eventName); }
        internal void Unset<T, U>(Action<T, U> action) { callee = (Action<T, U>)callee - action; if (callee == null || callee.GetInvocationList().Length <= 0) EventPool.Instance.Clear(eventName); }
        internal void Unset<T, U, V>(Action<T, U, V> action) { callee = (Action<T, U, V>)callee - action; if (callee == null || callee.GetInvocationList().Length <= 0) EventPool.Instance.Clear(eventName); }
        internal void Unset<T, U, V, W>(Action<T, U, V, W> action) { callee = (Action<T, U, V, W>)callee - action; if (callee == null || callee.GetInvocationList().Length <= 0) EventPool.Instance.Clear(eventName); }
        #endregion
    }
}

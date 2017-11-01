using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameWork
{
    /// Usage : EventListener.Do(XXX).When("");
    public class EventListener
    {
        ////////////////////////////////////////////////////
        // Statics
        ////////////////////////////////////////////////////
        #region Do
        public static EventAction Do(Action action)
        {
            return new EventAction().Set(action);
        }
        
        public static EventAction Do<T>(Action<T> action)
        {
            return new EventAction().Set(action);
        }

        public static EventAction Do<T, U>(Action<T, U> action)
        {
            return new EventAction().Set(action);
        }

        public static EventAction Do<T, U, V>(Action<T, U, V> action)
        {
            return new EventAction().Set(action);
        }
        
        public static EventAction Do<T, U, V, W>(Action<T, U, V, W> action)
        {
            return new EventAction().Set(action);
        }
        #endregion

        #region Undo
        public static void UndoAll()
        {
            EventPool.Instance.ClearAll();
        }

        // Clear events
        public static void Undo(string eventName)
        {
            EventPool.Instance.Clear(eventName);
        }

        public static void Undo(string eventName, Action action)
        {
            EventPool.Instance.Get(eventName).Unset(action);
        }

        public static void Undo<T>(string eventName, Action<T> action)
        {
            EventPool.Instance.Get(eventName).Unset(action);
        }

        public static void Undo<T, U>(string eventName, Action<T, U> action)
        {
            EventPool.Instance.Get(eventName).Unset(action);
        }

        public static void Undo<T, U, V>(string eventName, Action<T, U, V> action)
        {
            EventPool.Instance.Get(eventName).Unset(action);
        }

        public static void Undo<T, U, V, W>(string eventName, Action<T, U, V, W> action)
        {
            EventPool.Instance.Get(eventName).Unset(action);
        }
        #endregion
    }
}

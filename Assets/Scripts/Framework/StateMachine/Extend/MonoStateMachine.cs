using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    /// <summary>
    /// A specific state machine for MonoBehaviour.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class MonoStateMachine : MonoBehaviour, IStateMachine
    {
        protected Dictionary<string, IState> m_states = new Dictionary<string, IState>();
        protected Stack<string> m_stateStacks = new Stack<string>();

        public IState currentState = null;

        public virtual string initialStateName { get; protected set; }
        public string currentStateName
        {
            get
            {
                if (currentState != null)
                    return currentState.name;
                return "Null";
            }
        }

        public virtual void OnInit() { if (currentState == null && !string.IsNullOrEmpty(initialStateName)) ChangeState(initialStateName); }
        public virtual void OnUpdate()
        {
            if (currentState != null)
            {
                currentState.OnUpdate();
            }
        }
        public virtual void OnDestroy()
        {
            //if (currentState != null) currentState.OnExit();
            foreach (KeyValuePair<string, IState> pair in m_states)
            {
                EventListener.Undo(string.Format("ChangeState_{0}_{1}", name, pair.Key));
            }
            m_states.Clear();
            m_stateStacks.Clear();
        }

        #region State management
        public IState GetState(string stateName)
        {
            if (m_states.ContainsKey(stateName))
            {
                return m_states[stateName];
            }

            return null;
        }

        public void RegisterState(IState state)
        {
            if (!m_states.ContainsKey(state.name)) m_states.Add(state.name, state);

            state.OnRegistered(this);
            EventListener.Do(()=> { ChangeState(state.name); }).When(string.Format("ChangeState_{0}_{1}", name.Replace("(Clone)", ""), state.name));
        }

        public void ChangeState(string newStateName)
        {
            if (currentState != null)
            {
                if (currentState.name.Equals(newStateName))
                {
                    return;
                }
                currentState.OnExit();
            }

            IState newState = GetState(newStateName);

            // Debug
            //LogUtil.LogColor(LogUtil.Color.green, "[StateMachine] - [{0}] state changed : FROM [{1}] TO [{2}].", name, currentState == null ? "NULL" : currentState.name, newStateName);

            if (newState != null)
            {
                currentState = newState;
                currentState.OnEnter();
            }
        }

        public void PushState(string newStateName)
        {
            ChangeState(newStateName);
            m_stateStacks.Push(newStateName);
        }

        public void PopState()
        {
            string lastStateName = m_stateStacks.Pop();
            ChangeState(lastStateName);
        }
        #endregion

        // Use this to Register states
        public virtual void Awake() { }
        // change to the first State
        public virtual void Start()
        {
            OnInit();
        }

        public void Update()
        {
            OnUpdate();
        }

        #region Statics
        public static void DispatchStateChange(string stateMachineName, string stateName)
        {
            EventDispatcher.DispatchEvent(string.Format("ChangeState_{0}_{1}", stateMachineName, stateName));
        }
        #endregion
    }
}

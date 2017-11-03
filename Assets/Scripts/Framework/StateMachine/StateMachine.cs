using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameWork
{
    /// <summary>
    /// More generalized state machine. 
    /// The stateMachine pattern looks very much like the EntityComponent pattern, 
    /// only that a StateMachine can only have 1 running state, 
    /// while Entity has many components running at the same time.
    /// </summary>
    public abstract class StateMachine : IStateMachine
    {
        protected Dictionary<string, IState> m_states = new Dictionary<string, IState>();
        protected Stack<string> m_stateStacks = new Stack<string>();

        public IState currentState = null;

        public virtual string name { get { return "StateMachine"; } }
        public virtual string initialStateName { get; protected set; }

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
            currentState.OnExit();
            foreach (KeyValuePair<string, IState> pair in m_states)
            {
                EventListener.Undo(string.Format("ChangeState_{0}_{1}", name, pair.Key));
            }
            m_states.Clear();
            m_stateStacks.Clear();
        }

        #region State management
        protected IState GetState(string stateName)
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
            EventListener.Do(() => { ChangeState(state.name); }).When(string.Format("ChangeState_{0}_{1}", name, state.name));
        }

        public void ChangeState(string newStateName)
        {
            if (currentState != null)
            {
                currentState.OnExit();
            }

            IState newState = GetState(newStateName);
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

        #region State transition
        public void MakeTransition(ITransition transition)
        {

        }
        #endregion
        

        #region Statics
        public static void DispatchStateChange(string stateMachineName, string stateName)
        {
            EventDispatcher.DispatchEvent(string.Format("ChangeState_{0}_{1}", stateMachineName, stateName));
        }
        #endregion
    }
}

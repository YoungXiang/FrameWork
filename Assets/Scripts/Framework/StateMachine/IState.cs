using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameWork
{
    public abstract class IState
    {
        // name of this state
        public virtual string name { get { return "Null"; } }

        public abstract void OnRegistered(IStateMachine iStateMachine);
        public abstract void OnUpdate();
        public abstract void OnEnter();
        public abstract void OnExit();
    }

    public abstract class BaseState<T> : IState where T : class, IStateMachine
    {
        protected T stateMachine;
        public override void OnRegistered(IStateMachine iStateMachine)
        {
            stateMachine = iStateMachine as T;
        }

        public override void OnUpdate()
        {
        }

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
        }
    }
}

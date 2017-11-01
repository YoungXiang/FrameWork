using System;
using System.Collections.Generic;

namespace FrameWork
{
    public interface IStateMachine
    {
        void OnInit();
        void OnDestroy();
        void OnUpdate();

        void RegisterState(IState state);
        void ChangeState(string stateName);
        //void PushState(IState state);
        //void PopState(IState state);
    }
}

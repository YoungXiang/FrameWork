using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameWork
{
    public abstract class ITransition
    {
        public ITransition(IState a, IState b, float time) { }
        public virtual void Update(float dt) { }
    }
}

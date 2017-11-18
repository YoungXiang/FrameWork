using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public interface ISystem
    {
        void Execute();
        void ExecuteSingle(Entity ent);
    }

    public abstract class ESystem : ISystem
    {
        public ulong componentMask;

        public void OnEntityAdded(Entity ent)
        {

        }

        public void OnEntityRemoved(Entity ent)
        {

        }

        public void Execute()
        {
            EntityManager mgr = EntityManager.Instance;
            for (int i = 0; i < mgr.allActiveEntityIndices.Count; i++)
            {
                Entity ent = mgr.allEntities[mgr.allActiveEntityIndices[i]];
                if ((ent.componentMask & componentMask) == componentMask)
                {
                    ExecuteSingle(ent);
                }
            }
        }

        public abstract void ExecuteSingle(Entity ent);
    }

    public abstract class InitialSystem : ESystem
    {
        public InitialSystem()
        {
            //ESystemManager.Instance
        }
    }
}

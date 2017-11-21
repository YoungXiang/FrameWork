using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class Entity
    {
        internal static int G_UID = 0;
        /// <summary>
        /// entity unique id;
        /// </summary>
        public int uID;
        /// <summary>
        /// type of this entity
        /// </summary>
        public int eType;

        public DataListener data;
        public List<EComponent> components;

        // support maximum of 64 components in total!!!
        public ulong componentMask;

        public Entity()
        {
            uID = G_UID++;
            components = new List<EComponent>();
        }
        
        public void SetData(DataListener data_)
        {
            data = data_;
        }

        public void AddComponent<T>(EComponent com_)
        {
            componentMask |= com_.mask;
            components.Add(com_);

            // sort
            components.Sort((EComponent a, EComponent b) => 
            {
                return a.bit - b.bit;
            });
        }

        public T GetComponent<T>() where T : EComponent
        {
            int comBit = ComponentMask.ComponentBits[typeof(T)];
            int index = ComponentMask.ComponentIndex(componentMask, comBit);

            return (T)components[index];
        }

        public bool HasComponent<T>() where T : EComponent
        {
            int comBit = ComponentMask.ComponentBits[typeof(T)];
            return ((componentMask >> comBit) & 1) == 1;
        }

        public void Clear()
        {
            components.Clear();
        }
    }
}

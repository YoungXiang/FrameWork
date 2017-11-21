using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public abstract class EComponent
    {
        public const int MaxComponents = 64;

        public int bit;
        public ulong mask { get { return (ulong)(1 << bit); } }
    }
    
    public class ComponentMask
    {
        public static int TotalComponents = 0;
        public static int MaxComponentSize = 64; // bit length of ulong

        public static Dictionary<Type, int> ComponentBits = new Dictionary<Type, int>()
        {
            { typeof(EComponent), 0 },
        };
        
        public static int ComponentIndex(ulong mask, int componentPos)
        {
            ulong cm = (ulong)(1 << componentPos);
            if ((cm & mask) == 0) return -1;

            int bitPos = 0;
            while(componentPos >= 0)
            {
                if (((mask >> 1) & 1) == 1) bitPos++;
            }

            return bitPos;
        }
    }
}

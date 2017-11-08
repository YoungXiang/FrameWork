using System;
using System.Collections.Generic;
using MessagePack;

namespace FrameWork
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class TData<T> where T : class
    {
        public delegate bool SearchFilter(T val);
        public T[] datas;

        public T this[int index]
        {
            get
            {
                return datas[index];
            }
        }

        public T Search(SearchFilter filter)
        {
            for (int i = 0; i < datas.Length; i++)
            {
                if(filter(datas[i]))
                {
                    return datas[i];
                }
            }

            return null;
        }
    }

}

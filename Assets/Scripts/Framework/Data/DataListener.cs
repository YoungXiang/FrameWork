using System;
using System.Collections.Generic;

namespace FrameWork
{
    struct DataRange
    {
        public int min;
        public int max;
        public bool IsInRange(int val) { return val >= min && val < max; }

        public DataRange(int _min, int _max)
        {
            min = _min; max = _max;
        }

        public static DataRange IntRange = new DataRange(0, 10000);
        public static DataRange FloatRange = new DataRange(10000, 20000);
        public static DataRange BoolRange = new DataRange(20000, 30000);
        public static DataRange StringRange = new DataRange(30000, 40000);

        // you can add more range here
    }
    
    public class DataListener
    {
        #region Datas - Note that data is only allocated when registered.
        int[] intData;
        float[] floatData;
        bool[] boolData;
        string[] stringData;
        #endregion

        #region Key Lookups
        Dictionary<string, int> keyIndexMap = new Dictionary<string, int>();
        int[] indexRangeMap;

        void EnsureArray<T>(ref T[] _array, int _length)
        {
            if (_array == null) _array = new T[_length];
            else
            {
                if (_array.Length < _length)
                {
                    T[] copy = new T[_length];
                    Array.Copy(_array, copy, _array.Length);
                    _array = copy;
                }
            }
        }

        void GrowArray<T>(ref T[] _array, int _grow = 1)
        {
            if (_array == null) _array = new T[_grow];
            else
            {
                T[] copy = new T[_array.Length + _grow];
                Array.Copy(_array, copy, _array.Length);
                _array = copy;
            }
        }
        #endregion

        #region Push data onto stack(array)
        void PushInt(int _val)
        {
            GrowArray(ref intData);
            int dataIndex = intData.Length - 1;
            intData[dataIndex] = _val;  // stores the data

            int count = keyIndexMap.Count;
            EnsureArray(ref indexRangeMap, count);
            indexRangeMap[count - 1] = DataRange.IntRange.min + dataIndex;
        }
        void PushFloat(float _val)
        {
            GrowArray(ref floatData);
            int dataIndex = floatData.Length - 1;
            floatData[dataIndex] = _val;  // stores the data

            int count = keyIndexMap.Count;
            EnsureArray(ref indexRangeMap, count);
            indexRangeMap[count - 1] = DataRange.FloatRange.min + dataIndex;
        }
        void PushBool(bool _val)
        {
            GrowArray(ref boolData);
            int dataIndex = boolData.Length - 1;
            boolData[dataIndex] = _val;  // stores the data

            int count = keyIndexMap.Count;
            EnsureArray(ref indexRangeMap, count);
            indexRangeMap[count - 1] = DataRange.BoolRange.min + dataIndex;
        }
        void PushString(string _val)
        {
            GrowArray(ref stringData);
            int dataIndex = stringData.Length - 1;
            stringData[dataIndex] = _val;  // stores the data

            int count = keyIndexMap.Count;
            EnsureArray(ref indexRangeMap, count);
            indexRangeMap[count - 1] = DataRange.StringRange.min + dataIndex;
        }
        #endregion
        
        #region Registers
        void Register<T>(string _key, T _val)
        {
            if (!keyIndexMap.ContainsKey(_key))
            {
                int count = keyIndexMap.Count;
                keyIndexMap.Add(_key, count);
            }
        }
        public void RegisterAttr(string _key, int _val)
        {
            Register(_key, _val);
            PushInt(_val);
        }
        public void RegisterAttr(string _key, float _val)
        {
            Register(_key, _val);
            PushFloat(_val);
        }
        public void RegisterAttr(string _key, bool _val)
        {
            Register(_key, _val);
            PushBool(_val);
        }
        public void RegisterAttr(string _key, string _val)
        {
            Register(_key, _val);
            PushString(_val);
        }
        #endregion

        #region Getter & Setter
        /// <summary>
        /// Getter & Setter for int.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defautValue">This is a bad trick.</param>
        /// <returns></returns>
        public int this[string key, int defautValue]
        {
            get
            {
                int index = keyIndexMap[key];
                int range = indexRangeMap[index];
                int actIndex = range - DataRange.IntRange.min;
                return intData[actIndex];
            }
            set
            {
                int index = keyIndexMap[key];
                int range = indexRangeMap[index];
                int actIndex = range - DataRange.IntRange.min;
                intData[actIndex] = value;
                EventDispatcher.DispatchEvent(string.Format("{0}_{1}", DataName, key), value);
            }
        }
        public float this[string key, float defautValue]
        {
            get
            {
                int index = keyIndexMap[key];
                int range = indexRangeMap[index];
                int actIndex = range - DataRange.FloatRange.min;
                return floatData[actIndex];
            }
            set
            {
                int index = keyIndexMap[key];
                int range = indexRangeMap[index];
                int actIndex = range - DataRange.FloatRange.min;
                floatData[actIndex] = value;
                EventDispatcher.DispatchEvent(string.Format("{0}_{1}", DataName, key), value);
            }
        }
        public bool this[string key, bool defautValue]
        {
            get
            {
                int index = keyIndexMap[key];
                int range = indexRangeMap[index];
                int actIndex = range - DataRange.BoolRange.min;
                return boolData[actIndex];
            }
            set
            {
                int index = keyIndexMap[key];
                int range = indexRangeMap[index];
                int actIndex = range - DataRange.BoolRange.min;
                boolData[actIndex] = value;
                EventDispatcher.DispatchEvent(string.Format("{0}_{1}", DataName, key), value);
            }
        }
        public string this[string key, string defautValue]
        {
            get
            {
                int index = keyIndexMap[key];
                int range = indexRangeMap[index];
                int actIndex = range - DataRange.StringRange.min;
                return stringData[actIndex];
            }
            set
            {
                int index = keyIndexMap[key];
                int range = indexRangeMap[index];
                int actIndex = range - DataRange.StringRange.min;
                stringData[actIndex] = value;
                EventDispatcher.DispatchEvent(string.Format("{0}_{1}", DataName, key), value);
            }
        }
        #endregion

        #region Virtual
        public virtual string DataName { get { return "Base"; } }
        #endregion
    }
}

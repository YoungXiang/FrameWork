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
        public string name;
        public DataListener() { }
        public DataListener(string _name) { name = _name; }
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
            int count = keyIndexMap.Count;
            keyIndexMap.Add(_key, count);
        }
        public void RegisterAttr(string _key, int _val)
        {
            if (!keyIndexMap.ContainsKey(_key))
            {
                Register(_key, _val);
                PushInt(_val);
            }
        }
        public void RegisterAttr(string _key, float _val)
        {
            if (!keyIndexMap.ContainsKey(_key))
            {
                Register(_key, _val);
                PushFloat(_val);
            }
        }
        public void RegisterAttr(string _key, bool _val)
        {
            if (!keyIndexMap.ContainsKey(_key))
            {
                Register(_key, _val);
                PushBool(_val);
            }
        }
        public void RegisterAttr(string _key, string _val)
        {
            if (!keyIndexMap.ContainsKey(_key))
            {
                Register(_key, _val);
                PushString(_val);
            }
        }
        #endregion

        #region Getter & Setter
        public bool HasKey(string key)
        {
            return (keyIndexMap.ContainsKey(key));
        }
        int IntIndex(string key)
        {
            int index = keyIndexMap[key];
            int range = indexRangeMap[index];
            return range - DataRange.IntRange.min;
        }
        int FloatIndex(string key)
        {
            int index = keyIndexMap[key];
            int range = indexRangeMap[index];
            return range - DataRange.FloatRange.min;
        }
        int BoolIndex(string key)
        {
            int index = keyIndexMap[key];
            int range = indexRangeMap[index];
            return range - DataRange.BoolRange.min;
        }
        int StringIndex(string key)
        {
            int index = keyIndexMap[key];
            int range = indexRangeMap[index];
            return range - DataRange.StringRange.min;
        }
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
                if (!keyIndexMap.ContainsKey(key)) return defautValue;
                int actIndex = IntIndex(key);
                return intData[actIndex];
            }
            set
            {
                if (!keyIndexMap.ContainsKey(key)) return;
                int actIndex = IntIndex(key);
                if (value != intData[actIndex])
                {
                    intData[actIndex] = value;
                    DispatchOnValueChanged(key, value);
                }
            }
        }
        public float this[string key, float defautValue]
        {
            get
            {
                if (!keyIndexMap.ContainsKey(key)) return defautValue;
                int actIndex = FloatIndex(key);
                return floatData[actIndex];
            }
            set
            {
                if (!keyIndexMap.ContainsKey(key)) return;
                int actIndex = FloatIndex(key);
                if (value != floatData[actIndex])
                {
                    floatData[actIndex] = value;
                    DispatchOnValueChanged(key, value);
                }
            }
        }
        public bool this[string key, bool defautValue]
        {
            get
            {
                if (!keyIndexMap.ContainsKey(key)) return defautValue;
                int actIndex = BoolIndex(key);
                return boolData[actIndex];
            }
            set
            {
                if (!keyIndexMap.ContainsKey(key)) return;
                int actIndex = BoolIndex(key);
                if (value != boolData[actIndex])
                {
                    boolData[actIndex] = value;
                    DispatchOnValueChanged(key, value);
                }
            }
        }
        public string this[string key, string defautValue]
        {
            get
            {
                if (!keyIndexMap.ContainsKey(key)) return defautValue;
                int actIndex = StringIndex(key);
                return stringData[actIndex];
            }
            set
            {
                if (!keyIndexMap.ContainsKey(key)) return;
                int actIndex = StringIndex(key);
                if (value != stringData[actIndex])
                {
                    stringData[actIndex] = value;
                    DispatchOnValueChanged(key, value);
                }
            }
        }
        #endregion

        #region Virtual
        public virtual void DispatchOnValueChanged<T>(string key, T value)
        {
            EventDispatcher.DispatchEvent(string.Format("{0}_{1}", name, key), value);
        }
        #endregion
    }
}

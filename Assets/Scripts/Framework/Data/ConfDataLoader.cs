using System.IO;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

namespace FrameWork
{
    public interface ISerializedData
    {
        string name { get; }
        void Load();
    }

    /*
    [MessagePackObject(keyAsPropertyName: true)]
    public class TData<T> where T : class
    {
        public T[] datas;
    }

    public class TSData<T, U> : ISerializedData 
        where T : class 
        where U : class
    {
        public TData<T> serialized;

        protected string m_name;
        public string name { get { return m_name; } }
        public void Load()
        {
            string cached = string.Format("{0}/{1}.data", ConfDataLoader.localConfigPath, name);
            U u = IOUtils.DeserializeObjectFromFile<U>(cached);
            serialized = u as TData<T>;

            for (int i = 0; i < serialized.datas.Length; i++)
            {
                Debug.Log(serialized.datas[i]);
            }
        }

        public TSData(string name_) { m_name = name_; }
        
        public void Search()
        {
            //System.Array.BinarySearch < serialized.>
        }
        
        public T this[int id]
        {
            get
            {
                return serialized.datas[id];
            }
        }
    }
    */

    public class SerializedData<T> : ISerializedData where T:class
    {
        public T serialized;
        public bool dirty;
        public string name { get { return typeof(T).ToString().Remove(0, "FrameWork.".Length); } }

        public void Load()
        {
            string cached = string.Format("{0}/{1}.data", ConfDataLoader.localConfigPath, name);
            serialized = IOUtils.DeserializeObjectFromFile<T>(cached);
        }
    }

    public class ConfDataLoader : Singleton<ConfDataLoader>
    {
        public static readonly string localConfigPath = Path.Combine(Application.streamingAssetsPath, "ConfigDatas");
        protected Dictionary<string, ISerializedData> datas = new Dictionary<string, ISerializedData>();

        public override void Init()
        {
            //Add(new SerializedData<ClothCategoryData>());            
        }

        public void Add(ISerializedData data)
        {
            if (!datas.ContainsKey(data.name))
            {
                datas.Add(data.name, data);
            }
        }
        
        public T GetData<T>() where T : class
        {
            string name = typeof(T).ToString().Remove(0, "FrameWork.".Length);
            return GetData<T>(name);
        }

        T GetData<T>(string name) where T : class
        {
            if (datas.ContainsKey(name))
            {
                SerializedData<T> data = datas[name] as SerializedData<T>;
                return data.serialized;
            }

            Debug.AssertFormat(false, "SerializedData [{0}] is not loaded.", name);

            return null;
        }
        
        public void Load()
        {
            foreach(KeyValuePair<string, ISerializedData> pair in datas)
            {
                datas[pair.Key].Load();
            }
        }

    }
}


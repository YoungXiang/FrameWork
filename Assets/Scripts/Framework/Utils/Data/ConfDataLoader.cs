using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public interface ISerializedData
    {
        string name { get; }
        void Load();
    }

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
            //Add(new SerializedData<StageData>());
        }

        void Add(ISerializedData data)
        {
            if (!datas.ContainsKey(data.name))
            {
                datas.Add(data.name, data);
            }
        }
        
        public ISerializedData GetData(string name)
        {
            if (datas.ContainsKey(name))
            {
                return datas[name];
            }

            Debug.AssertFormat(false, "SerializedData [{0}] is not loaded.", name);

            return null;
        }

        public T GetData<T>(string name) where T : class
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


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class PrefabPool : SingleBehaviour<PrefabPool>
    {
        public class _Pool : Pool<GameObject>
        {
            public string prefabPath;

            public override GameObject NewInstance()
            {
                GameObject go = AssetManager.LoadPrefab(prefabPath);
                return go;
            }

            public override void ReleaseInstance(GameObject obj)
            {
                Object.Destroy(obj);
            }
        }


        private Dictionary<string, _Pool> prefabPool = new Dictionary<string, _Pool>();

        public GameObject Instantiate(string prefabPath)
        {
            if (!prefabPool.ContainsKey(prefabPath))
            {
                prefabPool.Add(prefabPath, new _Pool());
                prefabPool[prefabPath].prefabPath = prefabPath;
            }

            GameObject instance = prefabPool[prefabPath].Create();
            instance.SetActive(true);
            return instance;
        }

        public void Recycle(string prefabPath, GameObject instance)
        {
            if (prefabPool.ContainsKey(prefabPath))
            {
                prefabPool[prefabPath].Recycle(instance);
                instance.SetActive(false);
                instance.transform.SetParent(transform, false);
            }
        }

        public void Destroy(string prefabPath)
        {
            if (prefabPool.ContainsKey(prefabPath))
            {
                prefabPool[prefabPath].Destroy();
            }
        }
        
        public override void OnDestroy()
        {
            foreach(string prefab in prefabPool.Keys)
            {
                prefabPool[prefab].Destroy();
            }
            prefabPool.Clear();

            base.OnDestroy();
        }
    }
}

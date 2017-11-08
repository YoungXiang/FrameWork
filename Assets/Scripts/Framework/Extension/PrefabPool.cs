using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class PrefabPool : SingleBehaviour<PrefabPool>
    {
        public class _Pool : Pool<GameObject>
        {
            public GameObject prefab;

            public override GameObject NewInstance()
            {
                GameObject go = Object.Instantiate(prefab);
                return go;
            }

            public override void ReleaseInstance(GameObject obj)
            {
                Object.Destroy(obj);
            }
        }


        private Dictionary<GameObject, _Pool> prefabPool = new Dictionary<GameObject, _Pool>();

        public GameObject Instantiate(GameObject prefab)
        {
            if (!prefabPool.ContainsKey(prefab))
            {
                prefabPool.Add(prefab, new _Pool());
                prefabPool[prefab].prefab = prefab;
            }

            GameObject instance = prefabPool[prefab].Create();
            instance.SetActive(true);
            return instance;
        }

        public void Recycle(GameObject prefab, GameObject instance)
        {
            if (prefabPool.ContainsKey(prefab))
            {
                prefabPool[prefab].Recycle(instance);
                instance.SetActive(false);
                instance.transform.SetParent(transform, false);
            }
        }

        public void Destroy(GameObject prefab)
        {
            if (prefabPool.ContainsKey(prefab))
            {
                prefabPool[prefab].Destroy();
            }
        }
        
        public override void OnDestroy()
        {
            foreach(GameObject prefab in prefabPool.Keys)
            {
                prefabPool[prefab].Destroy();
            }
            prefabPool.Clear();

            base.OnDestroy();
        }
    }
}

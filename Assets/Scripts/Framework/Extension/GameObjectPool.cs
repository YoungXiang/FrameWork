using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class GameObjectPool : SingleBehaviour<GameObjectPool>
    {
        internal class _GameObjectPool : Pool<GameObject>
        {
            public Transform parent;
            public override GameObject NewInstance()
            {
                GameObject go = new GameObject("Pool_Unused");
                go.transform.SetParent(parent);
                go.SetActive(false);
                return go;
            }

            public override void ReleaseInstance(GameObject obj)
            {
                Object.Destroy(obj);
            }
        }

        _GameObjectPool _pool;
        bool isDestroyed = false;

        Dictionary<int, List<GameObject>> grouped = new Dictionary<int, List<GameObject>>();

        private void Awake()
        {
            _pool = new _GameObjectPool();
            _pool.parent = transform;
            _pool.Init(100, true);
        }

        public override void OnDestroy()
        {
            isDestroyed = true;
            _pool.Destroy();

            base.OnDestroy();
        }

        public GameObject Create(string name, int group = 0)
        {
            GameObject go = _pool.Create();
            go.name = name;
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            go.transform.SetParent(null);
            go.SetActive(true);

            if (!grouped.ContainsKey(group))
            {
                grouped.Add(group, new List<GameObject>());
            }
            grouped[group].Add(go);

            return go;
        }

        void RecycleInternal(GameObject go)
        {
            if (isDestroyed)
            {
                Destroy(go);
                return;
            }
            go.name = "Pool_Unused";
            go.transform.SetParent(transform);
            go.SetActive(false);

            Component[] components = go.GetComponents(typeof(Component));
            for (int i = 0; i < components.Length; i++)
            {
                if (!(components[i] is Transform))
                {
                    Debug.LogFormat("Trying to destroy component: {0}", components[i]);
                    Destroy(components[i]);
                }
            }

            _pool.Recycle(go);
        }

        public void Recycle(GameObject go, int group = 0)
        {
            if (grouped.ContainsKey(group))
            {
                grouped[group].Remove(go);
            }
            RecycleInternal(go);
        }

        public void Recycle(int group)
        {
            if (grouped.ContainsKey(group))
            {
                for (int i = 0; i < grouped[group].Count; i++)
                {
                    RecycleInternal(grouped[group][i]);
                }

                grouped[group].Clear();
                grouped.Remove(group);
            }
        }
    }

}

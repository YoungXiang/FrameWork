using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    #region Data Define
    public class CanvasListViewItemData
    {
        public string template;
        // we don't want to serialze this
        [System.NonSerialized]
        public MonoBehaviour item;
    }

    public class CanvasListViewItemNestedData<ChildType> : CanvasListViewItemData
    {
        public bool expanded;
        public ChildType[] children;
    }

    [System.Serializable]
    public class CanvasListViewItemInspectorData : CanvasListViewItemData
    {
    }
    #endregion

    #region Item Define
    public class CanvasListViewItemBase : MonoBehaviour
    {
    }

    public class CanvasListViewItem<DataType> : CanvasListViewItemBase where DataType : CanvasListViewItemData
    {
        // we don't want to serialze this
        [System.NonSerialized]
        public DataType data;

        public virtual void Setup(DataType data)
        {
            this.data = data;
            data.item = this;
        }
    }

    public class CanvasListViewItemTemplate
    {
        public readonly GameObject prefab;
        public readonly List<MonoBehaviour> pool = new List<MonoBehaviour>();

        public CanvasListViewItemTemplate(GameObject prefab)
        {
            if (prefab == null)
                Debug.LogError("Template prefab cannot be null");
            this.prefab = prefab;
        }
    }
    #endregion
}

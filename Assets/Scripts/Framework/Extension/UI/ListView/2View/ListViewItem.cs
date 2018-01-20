using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    /// <summary>
    /// Base class for all Item
    /// </summary>
    public class ListViewItemBase : MonoBehaviour
    {
    }

    /// <summary>
    /// Base Item class with DataType specified.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
    public class ListViewItem<DataType> : ListViewItemBase 
        where DataType : ListViewItemData
    {
        // Reference to Data
        [System.NonSerialized]
        public DataType data;
        
        // setup this Item using DataType
        public virtual void Setup(DataType data)
        {
            this.data = data;
            data.item = this;
        }
    }

    /// <summary>
    /// The template used to create an item.
    /// </summary>
    public class ListViewItemTemplate
    {
        public readonly GameObject prefab;
        public readonly Stack<MonoBehaviour> pool = new Stack<MonoBehaviour>();
        public Vector3 itemSize
        {
            get { return GetSize(); }
        }

        public ListViewItemTemplate(GameObject prefab)
        {
            if (null == prefab)
            {
                Debug.LogError("Template prefab cannot be null!");
            }

            this.prefab = prefab;
        }

        // return the size of this template
        protected virtual Vector3 GetSize()
        {
            var itemSize = Vector3.one;
            var rect = prefab.transform as RectTransform;
            if (rect != null)
            {
                Vector3 bound = rect.GetBounds().size;
                itemSize.x = bound.x;
                itemSize.y = bound.y;
                itemSize.z = 0.0f;
            }
            return itemSize;
        }
    }
}

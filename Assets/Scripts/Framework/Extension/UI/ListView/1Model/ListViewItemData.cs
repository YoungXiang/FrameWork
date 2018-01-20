using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    /// <summary>
    /// Base Data Type
    /// </summary>
    public class ListViewItemData
    {
        [Tooltip("Index in Controller's Templates.")]
        public int templateID;

        // Reference to Item
        [System.NonSerialized]
        public ListViewItemBase item;

        // Precomputed
        [System.NonSerialized]
        public float itemOffset;
        // Precomputed
        [System.NonSerialized]
        public float itemSize;
    }

    /// <summary>
    /// Base Nested Data Type
    /// </summary>
    /// <typeparam name="ChildType"></typeparam>
    public class ListViewItemNestedData<ChildType> : ListViewItemData
    {
        public bool expanded;
        public ChildType[] children;
    }

    /// <summary>
    /// Base Inspector Data Type
    /// </summary>
    [System.Serializable]
    public class ListViewItemInspectorData : ListViewItemData
    {

    }
}

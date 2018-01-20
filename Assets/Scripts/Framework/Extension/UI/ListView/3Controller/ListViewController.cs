using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    #region Abstract Base Controller
    /// <summary>
    /// Base class for ListViewController
    /// </summary>
    public abstract class ListViewControllerBase : UIBehaviour
    {
        [Tooltip("Either be horizontal or vertical")]
        public bool isHorizontal = true;

        [Tooltip("Distance we have scrolled from initial position")]
        // 滑动区间在[-(contentSize - viewSize), 0]
        public float scrollOffset;

        [Tooltip("Padding between items")]
        public int padding = 5;

        [Tooltip("Item template prefabs (as least one!)")]
        public GameObject[] templates;

        [NonSerialized]
        public bool isScrolling = false;
        [NonSerialized]
        public float contentSize = 0.0f;
        [NonSerialized]
        public float viewSize = 0.0f;

        #region Protected
        // 边界位置,可能是Left也可能是Top,这里统一命名为LeftSide
        protected Vector3 m_LeftSide;
        protected readonly List<ListViewItemTemplate> m_Templates = new List<ListViewItemTemplate>();
        protected RectTransform rect;
        #endregion

        #region Properties
        protected Vector3 axisVector3
        {
            get { return isHorizontal ? Vector3.left : Vector3.up; }
        }
        #endregion

        protected override void Awake()
        {
            base.Awake();
            Setup();
        }

        private void Update()
        {
            if (isScrolling) ViewUpdate();
        }

        protected virtual void Setup()
        {
            if (templates.Length < 1)
            {
                Debug.LogError("No templates!");
            }
            for (int i = 0; i < templates.Length; i++)
            {
                m_Templates.Add(new ListViewItemTemplate(templates[i]));
            }

            rect = transform as RectTransform;
            Vector3 bound = rect.GetBounds().size;
            viewSize = isHorizontal ? bound.x : bound.y;

            PreComputeConditions();
            // update once.
            ViewUpdate();
        }
        
        /// <summary>
        /// 更新View: 以scrollOffset驱动DataOffset，从而驱动Item
        /// </summary>
        internal virtual void ViewUpdate()
        {
            UpdateItems();
        }

        /// <summary>
        /// Pre compute all 'items' initial offset.
        /// </summary>
        protected abstract void PreComputeConditions();

        ///<summary>
        ///Calculate DataOffset by scrollOffset
        ///</summary> 
        protected abstract void UpdateItems();
        
        protected virtual void RecycleItem(int templateID, MonoBehaviour item)
        {
            if (item == null)
                return;
            if (templateID < 0 || templateID >= m_Templates.Count)
                return;

            m_Templates[templateID].pool.Push(item);
            item.gameObject.SetActive(false);
        }
    }
    #endregion

    public abstract class ListViewController<DataType, ItemType> : ListViewControllerBase
        where DataType : ListViewItemData
        where ItemType : ListViewItem<DataType>
    {
        // 由数组改为List，方便动态Add
        [Tooltip("Data Source")]
        public List<DataType> datas;
        
        protected int focusedIndex = 0;

        #region Overrides
        protected override void PreComputeConditions()
        {
            float initialOffset = 0.0f;
            for (int i = 0; i < datas.Count; i++)
            {
                datas[i].itemOffset = initialOffset;

                if (datas[i].templateID < 0 || datas[i].templateID >= m_Templates.Count)
                {
                    Debug.LogWarning("Cannot get item, template " + datas[i].templateID + " doesn't exist");
                    continue;
                }

                Vector3 templateItemSize = m_Templates[datas[i].templateID].itemSize;
                datas[i].itemSize = isHorizontal ? templateItemSize.x : templateItemSize.y;
                initialOffset += datas[i].itemSize + padding;
            }

            contentSize = initialOffset;
            m_LeftSide = transform.position + axisVector3 * viewSize * 0.5f;
        }

        protected override void UpdateItems()
        {
            for (int i = 0; i < datas.Count; i++)
            {
                float decide = datas[i].itemOffset + scrollOffset;
                //if (decide < 0 || decide > viewSize)
                //扩大回收判定边界
                if (decide < -datas[i].itemSize || decide > viewSize)
                {
                    // Recycle at Left Side or Right Side
                    RecycleItem(datas[i].templateID, datas[i].item);
                    datas[i].item = null;
                }
                else
                {
                    // List Middle
                    if (datas[i].item == null)
                    {
                        datas[i].item = GetItem(datas[i]);
                    }
                    datas[i].item.transform.position = m_LeftSide - axisVector3 * (decide + datas[i].itemSize * 0.5f);
                }
            }
        }
        #endregion
                
        protected ItemType GetItem(DataType data)
        {
            if (data == null)
            {
                Debug.LogWarning("Tried to get item with null data");
                return null;
            }
            if (data.templateID < 0 || data.templateID >= m_Templates.Count)
            {
                Debug.LogWarning("Cannot get item, template " + data.templateID + " doesn't exist");
                return null;
            }
            ItemType item = null;
            if (m_Templates[data.templateID].pool.Count > 0)
            {
                item = (ItemType)m_Templates[data.templateID].pool.Pop();
                
                item.gameObject.SetActive(true);
                item.Setup(data);
            }
            else
            {
                item = Instantiate(m_Templates[data.templateID].prefab, transform, false).GetComponent<ItemType>();
                item.Setup(data);
            }
            return item;
        }
        
        public virtual void ScrollNext()
        {
            ScrollTo(focusedIndex + 1);
        }

        public virtual void ScrollPrev()
        {
            ScrollTo(focusedIndex - 1);
        }

        public virtual void ScrollTo(int index)
        {
            index = Mathf.Clamp(index, 0, datas.Count - 1);

            focusedIndex = index;
            scrollOffset = Mathf.Clamp(-datas[index].itemOffset, -(contentSize - viewSize), 0);
            ViewUpdate();
        }

        public virtual void AddData(DataType data)
        {
            if (data.templateID < 0 || data.templateID >= m_Templates.Count)
            {
                Debug.LogWarning("Cannot get item, template " + data.templateID + " doesn't exist");
                return;
            }

            Vector3 templateItemSize = m_Templates[data.templateID].itemSize;
            data.itemSize = isHorizontal ? templateItemSize.x : templateItemSize.y;

            float itemPaddedSize = data.itemSize + padding;
            data.itemOffset = datas[datas.Count - 1].itemOffset + itemPaddedSize;
            contentSize += itemPaddedSize;

            datas.Add(data);

            ScrollTo(datas.Count - 1);
        }

        public virtual void RemoveData(int index)
        {
            if (index == datas.Count - 1)
            {
                // remove last
                DataType data = datas[index];
                RecycleItem(data.templateID, data.item);
                contentSize -= data.itemSize + padding;
                datas.RemoveAt(index);

                ScrollTo(datas.Count - 1);
            }
            else
            {
                if (index < 0 || index >= datas.Count)
                {
                    return;
                }

                RecycleItem(datas[index].templateID, datas[index].item);
                datas.RemoveAt(index);
                SetDataDirty();
                // scroll to next
                ScrollTo(index + 1);
            }
        }
        
        public void SetDataDirty()
        {
            PreComputeConditions();
        }
    }
}

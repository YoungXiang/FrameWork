using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace FrameWork
{
    public abstract class CanvasListViewControllerBase : UIBehaviour
    {
        [Tooltip("Either be horizontal or vertical")]
        public bool horizontal = true;
        [Tooltip("Distance we have scrolled from initial position")]
        public float scrollOffset;
        [Tooltip("Padding (in pixels) between items")]
        public int padding = 5;
        [Tooltip("Item temlate prefabs (at least one is required)")]
        public GameObject[] templates;

        [System.NonSerialized]
        public bool isScrolling = false;

        //Protected variables
        protected int m_DataOffset;
        protected int m_NumItems;
        protected Vector3 m_LeftSide;
        protected Vector3 m_ItemSize;
        protected readonly Dictionary<string, CanvasListViewItemTemplate> m_Templates = new Dictionary<string, CanvasListViewItemTemplate>();

        protected float range;
        protected RectTransform rect;

        //Public properties
        public Vector3 itemSize
        {
            get { return m_ItemSize; }
        }

        //Protected properties
        protected Vector3 axisVector3
        {
            get { return horizontal ? Vector3.left : Vector3.up; }
        }

        protected float itemAxisRange
        {
            get { return horizontal ? m_ItemSize.x : m_ItemSize.y; }
        }

        protected override void Start()
        {
            base.Start();
            Setup();
        }

        void Update()
        {
            if (isScrolling) ViewUpdate();
        }

        protected virtual void Setup()
        {
            if (templates.Length < 1)
            {
                Debug.LogError("No templates!");
            }
            foreach (var template in templates)
            {
                if (m_Templates.ContainsKey(template.name))
                    Debug.LogError("Two templates cannot have the same name");
                m_Templates[template.name] = new CanvasListViewItemTemplate(template);
            }

            rect = transform as RectTransform;
            Vector3 bound = rect.GetBounds().size;
            range = horizontal ? bound.x : bound.y;

            PreComputeConditions();
            // update once.
            ViewUpdate();
        }

        protected virtual void ViewUpdate()
        {
            ComputeConditions();
            UpdateItems();
        }

        protected virtual void PreComputeConditions()
        {
            if (templates.Length > 0)
            {
                //Use first template to get item size
                m_ItemSize = GetObjectSize(templates[0]);
            }

            //Resize range to nearest multiple of item width
            m_NumItems = Mathf.RoundToInt(range / itemAxisRange); 
            range = m_NumItems * itemAxisRange;

            // Add plus 1, so that our list will behave properly
            m_NumItems++;

            //Get initial conditions.
            //Make sure the left side add 1 more itemAxisRange so that it behaves properly
            m_LeftSide = transform.position + axisVector3 * (range + itemAxisRange) * 0.5f;
        }

        protected virtual void ComputeConditions()
        {
            m_DataOffset = (int)(scrollOffset / itemAxisRange);
            //if (scrollOffset < 0) m_DataOffset--;

            //Debug.LogFormat("ScrollOffset : {0}, itemSize : {1}, dataOffset : {2}", 
            //    scrollOffset, itemAxisRange, m_DataOffset);
        }

        protected abstract void UpdateItems();

        public virtual void ScrollNext()
        {
            scrollOffset += itemAxisRange;
        }

        public virtual void ScrollPrev()
        {
            scrollOffset -= itemAxisRange;
        }

        public virtual void ScrollTo(int index)
        {
            scrollOffset = index * itemAxisRange;
        }

        protected virtual void Positioning(Transform t, int offset)
        {
            t.position = m_LeftSide + ((offset + 1) * itemAxisRange + scrollOffset) * (-axisVector3);
        }

        protected virtual Vector3 GetObjectSize(GameObject g)
        {
            var itemSize = Vector3.one;
            var rect = g.transform as RectTransform;
            if (rect != null)
            {
                Vector3 bound = rect.GetBounds().size;
                itemSize.x = bound.x + padding;
                itemSize.y = bound.y + padding;
                itemSize.z = 0.0f;
            }
            return itemSize;
        }

        protected virtual void RecycleItem(string template, MonoBehaviour item)
        {
            if (item == null || template == null)
                return;
            m_Templates[template].pool.Add(item);
            item.gameObject.SetActive(false);
        }
    }

    public abstract class CanvasListViewController<DataType, ItemType> : CanvasListViewControllerBase
        where DataType : CanvasListViewItemData
        where ItemType : CanvasListViewItem<DataType>
    {
        [Tooltip("Source Data")]
        public DataType[] data;

        protected override void UpdateItems()
        {
            for (int i = 0; i < data.Length; i++)
            {
                // In order to make the view looks correct, we recycle the first element when it's completely out of bounds.
                //if (i + m_DataOffset < 0)
                if (i + m_DataOffset < 0)
                {
                    ExtremeLeft(data[i]);
                }
                else if (i + m_DataOffset > m_NumItems - 1)
                {
                    ExtremeRight(data[i]);
                }
                else
                {
                    ListMiddle(data[i], i);
                }
            }
        }

        protected virtual void ExtremeLeft(DataType data)
        {
            RecycleItem(data.template, data.item);
            data.item = null;
        }

        protected virtual void ExtremeRight(DataType data)
        {
            RecycleItem(data.template, data.item);
            data.item = null;
        }

        protected virtual void ListMiddle(DataType data, int offset)
        {
            if (data.item == null)
            {
                data.item = GetItem(data);
            }
            Positioning(data.item.transform, offset);
        }

        protected virtual ItemType GetItem(DataType data)
        {
            if (data == null)
            {
                Debug.LogWarning("Tried to get item with null data");
                return null;
            }
            if (!m_Templates.ContainsKey(data.template))
            {
                Debug.LogWarning("Cannot get item, template " + data.template + " doesn't exist");
                return null;
            }
            ItemType item = null;
            if (m_Templates[data.template].pool.Count > 0)
            {
                item = (ItemType)m_Templates[data.template].pool[0];
                m_Templates[data.template].pool.RemoveAt(0);

                item.gameObject.SetActive(true);
                item.Setup(data);
            }
            else
            {
                item = Instantiate(m_Templates[data.template].prefab, transform, false).GetComponent<ItemType>();
                item.Setup(data);
            }
            return item;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListView
{
    public class CanvasListView<DataType, ItemType> : ListViewController<DataType, ItemType>
        where DataType : ListViewItemNestedData<DataType>
        where ItemType : ListViewItem<DataType>
    {
        [Tooltip("True:horizontal. False:vertical.")]
        public bool isHorizontal = true;

        // overrited
        protected new float range = 1;

        // rect transform the ListView is attached to
        protected RectTransform rect;

        readonly Dictionary<string, Vector3> m_TemplateSizes = new Dictionary<string, Vector3>();
        float m_ScrollReturn = float.MaxValue;

        //static readonly Vector3 dir = new Vector3(-1, 1, 0);

        int axis = 0;
        int inverseAxis = 1;
        Vector3 dir = Vector3.left;

        protected override void Setup()
        {
            rect = GetComponent<RectTransform>();
            range = rect.rect.height;

            base.Setup();
            foreach(var kvp in m_Templates)
            {
                m_TemplateSizes[kvp.Key] = GetObjectSize(kvp.Value.prefab);
            }

            SetupData();
        }

        /// <summary>
        /// Fill the source data : YourItemDataType[] data
        /// </summary>
        protected virtual void SetupData()
        {

        }

        protected override void ViewUpdate()
        {
            axis = isHorizontal ? 0 : 1;
            inverseAxis = Mathf.Abs(axis - 1);
            dir = isHorizontal ? Vector3.left : Vector3.up;

            base.ViewUpdate();
        }

        protected override void ComputeConditions()
        {
            if (templates.Length > 0)
            {
                if (templates.Length > 0)
                {
                    //Use first template to get item size
                    m_ItemSize = GetObjectSize(templates[0]);
                }
                //Resize range to nearest multiple of item width
                m_NumItems = Mathf.RoundToInt(range / m_ItemSize[axis]); //Number of cards that will fit
                range = m_NumItems * m_ItemSize[axis];

                //Get initial conditions. This procedure is done every frame in case the collider bounds change at runtime
                m_LeftSide = transform.position + dir * range * 0.5f; // + Vector3.left * itemSize.x * 0.5f;

                m_DataOffset = (int)(scrollOffset / itemSize[axis]);
                if (scrollOffset < 0)
                    m_DataOffset--;
            }
        }

        protected override void UpdateItems()
        {
            float totalOffset = 0;
            UpdateRecursively(data, ref totalOffset);
            totalOffset -= m_ItemSize[axis];
            if (totalOffset < -scrollOffset)
            {
                m_ScrollReturn = -totalOffset;
            }
        }

        void UpdateRecursively(DataType[] data, ref float totalOffset)
        {
            foreach (var item in data)
            {
                float itemSpace = m_TemplateSizes[item.template][axis];
                if (totalOffset + scrollOffset + itemSpace < 0)
                {
                    ExtremeLeft(item);
                }
                else if (totalOffset + scrollOffset > range)
                {
                    ExtremeRight(item);
                }
                else
                {
                    ListMiddle(item, totalOffset + scrollOffset);
                }
                totalOffset += itemSpace;
                if (item.children != null)
                {
                    if (item.expanded)
                    {
                        UpdateRecursively(item.children, ref totalOffset);
                    }
                    else
                    {
                        RecycleChildren(item);
                    }
                }
            }
        }

        void ListMiddle(DataType data, float offset)
        {
            if (data.item == null)
            {
                data.item = GetItem(data);
            }
            Positioning(data.item.transform, offset);
        }

        void Positioning(Transform t, float offset)
        {
            t.position = m_LeftSide + (offset + (itemSize[axis]) * 0.5f) * (-dir);
        }

        protected override Vector3 GetObjectSize(GameObject g)
        {
            Vector3 itemSize = Vector3.one;
            //TODO: Better method for finding object size
            RectTransform rect = g.GetComponent<RectTransform>();
            if (rect != null)
            {
                itemSize.x = rect.rect.width + padding;
                itemSize.y = rect.rect.height + padding;
                itemSize.z = 0.0f;
            }

            return itemSize;
        }

        public void OnStopScrolling()
        {
            if (scrollOffset > 0)
            {
                scrollOffset = 0;
            }
            if (m_ScrollReturn < float.MaxValue)
            {
                scrollOffset = m_ScrollReturn;
                m_ScrollReturn = float.MaxValue;
            }
        }

        void RecycleChildren(DataType data)
        {
            foreach (var child in data.children)
            {
                RecycleItem(child.template, child.item);
                child.item = null;
                if (child.children != null)
                    RecycleChildren(child);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    public class GridViewController<DataType, ItemType> : ListViewController<DataType, ItemType>
        where DataType : ListViewItemData
        where ItemType : ListViewItem<DataType>
    {
        [Tooltip("FixedRowCount or FixedColumnCount depends on isHorizontal or not.")]
        public int constraintCount = 1;

        public Vector2 cellSize;

        protected override void PreComputeConditions()
        {
            float initialOffset = 0.0f;
            for (int i = 0; i < datas.Count; i += constraintCount)
            {
                for (int j = 0; j < constraintCount; j++)
                {
                    int index = i + j;
                    if (index < datas.Count)
                    {
                        DataType data = datas[index];
                        data.itemOffset = initialOffset;
                        data.itemSize = isHorizontal ? cellSize.x : cellSize.y;
                    }
                }

                initialOffset += isHorizontal ? cellSize.x : cellSize.y + padding;
            }
            contentSize = initialOffset;
            m_LeftSide = transform.position + axisVector3 * viewSize * 0.5f;

            if (isHorizontal) m_LeftSide.y += (constraintCount - 1) * (cellSize.y + padding) * 0.5f;
            else m_LeftSide.x -= (constraintCount - 1) * (cellSize.x + padding) * 0.5f;
        }

        protected override void UpdateItems()
        {
            Vector3 constraintPos = Vector3.zero;
            for (int i = 0; i < datas.Count; i += constraintCount)
            {
                if (isHorizontal)
                {
                    constraintPos.y = 0;
                }
                else
                {
                    constraintPos.x = 0;
                }
                for (int j = 0; j < constraintCount; j++)
                {
                    int index = i + j;
                    if (index < datas.Count)
                    {
                        DataType data = datas[index];
                        float decide = data.itemOffset + scrollOffset;
                        if (decide < -data.itemSize || decide > viewSize)
                        {
                            // Recycle if out of bounds
                            RecycleItem(data.templateID, data.item);
                            data.item = null;
                        }
                        else
                        {
                            // List Middle
                            if (data.item == null)
                            {
                                data.item = GetItem(data);
                            }

                            if (isHorizontal)
                            {
                                constraintPos.y = -(j * (cellSize.y + padding));
                            }
                            else
                            {
                                constraintPos.x = (j * (cellSize.y + padding));
                            }
                            Vector3 offset = -axisVector3 * (decide + data.itemSize * 0.5f + padding) + constraintPos;
                            data.item.transform.position = m_LeftSide + offset;
                        }
                    }
                }
            }
        }
    }
}

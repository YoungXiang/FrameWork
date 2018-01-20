using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(ListViewControllerBase))]
    public class ListViewInputHandler : MonoBehaviour,
        IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public ListViewControllerBase listView;

        [SerializeField]
        protected float m_Elasticity = 0.1f;

        [SerializeField]
        protected float m_Damp = 0.135f;

        protected bool m_IsScrolling = false;
        protected bool m_IsAligning = false;
        protected Vector3 m_StartPosition;
        protected float m_StartTime;
        protected float m_StartOffset;
        protected float m_AlignOffset = 0.0f;
        protected float m_Speed = 0.0f;

        protected const float Epsilon = 2f;

        /*
        protected ListViewInputHandler()
        {
            useLegacyMeshGeneration = false;
        }

        // 这里只是实现一个能收到事件但不绘制的控件
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
        }
        */

        private void Awake()
        {
            if (null == listView)
            {
                listView = GetComponent<ListViewControllerBase>();
            }
        }

        private void LateUpdate()
        {
            UpdateAutoScrolling();
        }

        #region Scroll Control
        protected void InitScroll()
        {
            m_Speed = 0.0f;
        }

        protected virtual void StartScrolling(Vector3 start)
        {
            if (m_IsScrolling) return;

            listView.isScrolling = true;
            m_IsScrolling = true;
            m_StartPosition = start;
            m_StartOffset = listView.scrollOffset;
            m_StartTime = Time.unscaledTime;
        }

        protected virtual void Scroll(Vector3 position)
        {
            if (m_IsScrolling)
            {
                Vector3 delta = position - m_StartPosition;
                float lastOffset = listView.scrollOffset;
                listView.scrollOffset = m_StartOffset + (listView.isHorizontal ? RubberDelta(delta.x, listView.viewSize) : -RubberDelta(delta.y, listView.viewSize));

                float deltaTime = Time.unscaledDeltaTime;
                float newSpeed = (listView.scrollOffset - lastOffset) / deltaTime;
                m_Speed = Mathf.Lerp(m_Speed, newSpeed, deltaTime * 10);                
            }
        }

        protected virtual void StopScrolling()
        {
            //listView.isScrolling = false;
            m_IsScrolling = false;
            CheckShouldAlign();
        }

        protected virtual void UpdateAutoScrolling()
        {
            if (m_IsScrolling) return;
            if (!listView.isScrolling) return;

            // Update Auto Scrolling
            float timeDelta = Time.unscaledDeltaTime;
            if (m_IsAligning)
            {
                if (Mathf.Abs(m_Speed) < Epsilon)
                {
                    m_Speed = 0.0f;
                    listView.isScrolling = false;
                    listView.scrollOffset = m_AlignOffset;
                    listView.ViewUpdate();
                    return;
                }
                listView.scrollOffset = Mathf.SmoothDamp(listView.scrollOffset, m_AlignOffset, ref m_Speed, m_Elasticity, float.PositiveInfinity, timeDelta);
            }
            else if (m_Speed != 0.0f)
            {
                m_Speed *= Mathf.Pow(m_Damp, timeDelta);
                if (Mathf.Abs(m_Speed) < Epsilon)
                {
                    m_Speed = 0.0f;
                    listView.isScrolling = false;
                    return;
                }
                listView.scrollOffset = Mathf.Clamp(listView.scrollOffset + m_Speed * timeDelta, -(listView.contentSize - listView.viewSize), 0);
            }
        }

        private void CheckShouldAlign()
        {
            m_IsAligning = false;
            // only auto move when over streching
            if (listView.scrollOffset > 0.0f)
            {
                // align left
                m_AlignOffset = 0.0f;
                m_Speed = listView.scrollOffset / m_Elasticity;
                m_IsAligning = true;
            }
            else
            {
                float contentRemainSize = listView.contentSize - Mathf.Abs(listView.scrollOffset);
                if (contentRemainSize < listView.viewSize)
                {
                    // align right
                    m_AlignOffset = Mathf.Min(0, -(listView.contentSize - listView.viewSize));
                    m_Speed = Mathf.Abs(m_AlignOffset - listView.scrollOffset) / m_Elasticity;
                    m_IsAligning = true;
                }
            }
        }

        private float RubberDelta(float overStretching, float viewSize)
        {
            return (1 - (1 / ((Mathf.Abs(overStretching) * 0.55f / viewSize) + 1))) * viewSize * Mathf.Sign(overStretching);
        }
        #endregion

        #region Interface Implementation
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            InitScroll();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            StartScrolling(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            StopScrolling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            Scroll(eventData.position);
        }
        #endregion
    }
}

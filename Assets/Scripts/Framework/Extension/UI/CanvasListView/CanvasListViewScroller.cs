using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FrameWork
{
    [RequireComponent(typeof(CanvasListViewControllerBase))]
    public class CanvasListViewScroller : CanvasListViewInputHandler, 
        IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        protected bool m_Scrolling;
        protected Vector3 m_StartPosition;
        protected float m_StartOffset;

        protected virtual void Awake()
        {
            if (listView == null) listView = GetComponent<CanvasListViewControllerBase>();
        }

        /*
        protected override void HandleInput()
        {

        }
        */

        protected virtual void StartScrolling(Vector3 start)
        {
            if (m_Scrolling) return;

            listView.isScrolling = true;
            m_Scrolling = true;
            m_StartPosition = start;
            m_StartOffset = listView.scrollOffset;
        }

        protected virtual void Scroll(Vector3 position)
        {
            if (m_Scrolling)
                listView.scrollOffset = m_StartOffset + position.x - m_StartPosition.x;
        }

        protected virtual void StopScrolling()
        {
            listView.isScrolling = false;
            m_Scrolling = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            //Debug.LogFormat("OnBeginDrag : {0}", eventData.position);
            StartScrolling(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //Debug.LogFormat("OnEndDrag : {0}", eventData.position);
            StopScrolling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            //Debug.LogFormat("OnDrag : {0}", eventData.position);
            Scroll(eventData.position);
        }
    }
}
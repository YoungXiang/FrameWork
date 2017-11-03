using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace FrameWork
{
    [AddComponentMenu("UI/ProgressBar", 100)]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class ProgressBar : UIBehaviour
    {
        [SerializeField]
        private RectTransform m_fill;   // the foreground transform
        public RectTransform fill { get { return m_fill; } set { m_fill = value; } }

        [System.NonSerialized]
        private RectTransform m_Rect;
        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        // total progress
        [SerializeField]
        protected float m_progressTotal;
        public float progressTotal
        {
            get { return m_progressTotal; }
            set { m_progressTotal = value; UpdateView(); }
        }

        // fill progress
        [SerializeField]
        protected float m_progressFill;
        public float progressFill
        {
            get { return m_progressFill; }
            set
            {
                m_progressFill = Mathf.Clamp(value, 0, m_progressTotal);
                UpdateView();
            }
        }

        protected ProgressBar()
        {
            m_progressTotal = 1.0f;
            m_progressFill = 0.0f;
        }

        protected void UpdateView()
        {
            m_fill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.rect.width * (m_progressFill / m_progressTotal));
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            UpdateView();
        }
#endif
    }

}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork
{
    /// <summary>
    /// Labels are graphics that display text with string id
    /// </summary>
    [AddComponentMenu("UI/StringTableText", 102)]
    public class StringTableText : Text
    {
        /// <summary>
        /// For localization
        /// </summary>
        //[TextArea(1, 1)]
        [SerializeField]
        protected string m_StringTableID = String.Empty;
        public string StringTableID
        {
            get { return m_StringTableID; }
            set
            {
                m_StringTableID = value;
                OnStringTableIDChanged();
            }
        }

        public int test;

        protected StringTableText() : base()
        {
        }

        void OnStringTableIDChanged()
        {
            if (!string.IsNullOrEmpty(m_StringTableID))
            {
                //text = XlsDataUtils.GetString(m_StringTableID);
            }
            else
            {
                //text = "";
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            OnStringTableIDChanged();
        }

#endif // if UNITY_EDITOR
    }
}

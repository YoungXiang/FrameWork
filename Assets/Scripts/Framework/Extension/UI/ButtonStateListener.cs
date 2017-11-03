using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrameWork
{
    [RequireComponent(typeof(Button))]
    public class ButtonStateListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public UnityAction onButtonPressed;
        public UnityAction onButtonReleased;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (onButtonPressed != null) onButtonPressed.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (onButtonReleased != null) onButtonReleased.Invoke();
        }
    }
}

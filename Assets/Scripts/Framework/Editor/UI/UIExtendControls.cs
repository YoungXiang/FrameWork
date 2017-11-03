using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork
{
    public static class UIExtendControls
    {
        public struct Resources
        {
            public Sprite standard;
            public Sprite background;
            public Sprite inputField;
            public Sprite knob;
            public Sprite checkmark;
            public Sprite dropdown;
            public Sprite mask;
            public Material materialDefault;
            public Material materialDefaultFont;
        }

        internal const float kWidth = 160f;
        internal const float kThickHeight = 30f;
        internal const float kThinHeight = 20f;
        internal static Vector2 s_ThickElementSize = new Vector2(kWidth, kThickHeight);
        internal static Vector2 s_ThinElementSize = new Vector2(kWidth, kThinHeight);
        internal static Vector2 s_ImageElementSize = new Vector2(100f, 100f);
        internal static Color s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);
        internal static Color s_PanelColor = new Color(1f, 1f, 1f, 0.392f);
        internal static Color s_TextColor = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);

        // Helper methods at top

        internal static GameObject CreateUIElementRoot(string name, Vector2 size)
        {
            GameObject child = new GameObject(name);
            RectTransform rectTransform = child.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            return child;
        }

        internal static GameObject CreateUIObject(string name, GameObject parent)
        {
            GameObject go = new GameObject(name);
            go.AddComponent<RectTransform>();
            SetParentAndAlign(go, parent);
            return go;
        }

        internal static void SetDefaultTextValues(Text lbl)
        {
            // Set text values we want across UI elements in default controls.
            // Don't set values which are the same as the default values for the Text component,
            // since there's no point in that, and it's good to keep them as consistent as possible.
            lbl.color = s_TextColor;
        }

        internal static void SetDefaultColorTransitionValues(Selectable slider)
        {
            ColorBlock colors = slider.colors;
            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
            colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
            colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
        }

        internal static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;

            child.transform.SetParent(parent.transform, false);
            SetLayerRecursively(child, parent.layer);
        }

        internal static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }

        internal static void SetRectTransformFill(GameObject go)
        {
            RectTransform rectTransform = go.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
        }

        internal static void SetRectTransformFillLeft(GameObject go)
        {
            RectTransform rectTransform = go.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.pivot = new Vector2(0, 0.5f);
        }

        // Actual controls
        public static GameObject CreateProgressBar(Resources resources)
        {
            GameObject root = CreateUIElementRoot("ProgressBar", s_ThickElementSize);
            ProgressBar progressBar = root.AddComponent<ProgressBar>();
            
            GameObject backImage = new GameObject("Background");
            {
                Image back = backImage.AddComponent<Image>();
                back.sprite = resources.background;
                back.type = Image.Type.Sliced;
                back.color = s_DefaultSelectableColor;
                back.raycastTarget = false;
                back.material = resources.materialDefault;
                SetRectTransformFill(backImage);
            }
            SetParentAndAlign(backImage, root);

            GameObject mask = new GameObject("Mask");
            {
                Image image = mask.AddComponent<Image>();
                image.sprite = resources.standard;
                image.type = Image.Type.Sliced;
                image.color = Color.black;      // black as mask's color
                image.raycastTarget = false;
                image.material = resources.materialDefault;
                SetRectTransformFill(mask);

                Mask mk = mask.AddComponent<Mask>();
            }
            SetParentAndAlign(mask, root);

            GameObject fillImage = new GameObject("Fill");
            {
                Image image = fillImage.AddComponent<Image>();
                image.sprite = resources.standard;
                image.type = Image.Type.Sliced;
                image.color = s_DefaultSelectableColor;
                image.raycastTarget = false;
                image.material = resources.materialDefault;
                SetRectTransformFillLeft(fillImage);

                progressBar.fill = image.rectTransform;
            }
            SetParentAndAlign(fillImage, mask);

            /*
            GameObject childText = new GameObject("Percentage Text");
            {
                Text text = childText.AddComponent<Text>();
                text.text = "0%";
                text.alignment = TextAnchor.MiddleCenter;
                text.raycastTarget = false;
                text.material = resources.materialDefaultFont;
                SetDefaultTextValues(text);
                SetRectTransformFill(childText);
            }
            SetParentAndAlign(childText, mask);
            */

            progressBar.progressFill = 0.0f;
            return root;
        }
        
    }
}

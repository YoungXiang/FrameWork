using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace FrameWork
{
    public static class DefaultControls
    {
        // Actual controls
        public static GameObject CreateButton(UIExtendControls.Resources resources)
        {
            GameObject buttonRoot = UIExtendControls.CreateUIElementRoot("Button", UIExtendControls.s_ThickElementSize);

            GameObject childText = new GameObject("Text");
            UIExtendControls.SetParentAndAlign(childText, buttonRoot);

            Image image = buttonRoot.AddComponent<Image>();
            image.sprite = resources.standard;
            image.type = Image.Type.Sliced;
            image.color = UIExtendControls.s_DefaultSelectableColor;

            Button bt = buttonRoot.AddComponent<Button>();
            UIExtendControls.SetDefaultColorTransitionValues(bt);

            // the magic is here
            StringTableText text = childText.AddComponent<StringTableText>();
            text.text = "Button";
            text.alignment = TextAnchor.MiddleCenter;
            UIExtendControls.SetDefaultTextValues(text);

            RectTransform textRectTransform = childText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;

            return buttonRoot;
        }

        public static GameObject CreateText(UIExtendControls.Resources resources)
        {
            GameObject go = UIExtendControls.CreateUIElementRoot("Text", UIExtendControls.s_ThickElementSize);

            StringTableText lbl = go.AddComponent<StringTableText>();
            lbl.text = "New Text";
            lbl.raycastTarget = false;
            UIExtendControls.SetDefaultTextValues(lbl);

            return go;
        }
        
        public static GameObject CreateToggle(UIExtendControls.Resources resources)
        {
            // Set up hierarchy
            GameObject toggleRoot = UIExtendControls.CreateUIElementRoot("Toggle", UIExtendControls.s_ThinElementSize);

            GameObject background = UIExtendControls.CreateUIObject("Background", toggleRoot);
            GameObject checkmark = UIExtendControls.CreateUIObject("Checkmark", background);
            GameObject childLabel = UIExtendControls.CreateUIObject("Label", toggleRoot);

            // Set up components
            Toggle toggle = toggleRoot.AddComponent<Toggle>();
            toggle.isOn = true;

            Image bgImage = background.AddComponent<Image>();
            bgImage.sprite = resources.standard;
            bgImage.type = Image.Type.Sliced;
            bgImage.color = UIExtendControls.s_DefaultSelectableColor;

            Image checkmarkImage = checkmark.AddComponent<Image>();
            checkmarkImage.sprite = resources.checkmark;

            StringTableText label = childLabel.AddComponent<StringTableText>();
            label.text = "Toggle";
            UIExtendControls.SetDefaultTextValues(label);

            toggle.graphic = checkmarkImage;
            toggle.targetGraphic = bgImage;
            UIExtendControls.SetDefaultColorTransitionValues(toggle);

            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin        = new Vector2(0f, 1f);
            bgRect.anchorMax        = new Vector2(0f, 1f);
            bgRect.anchoredPosition = new Vector2(10f, -10f);
            bgRect.sizeDelta        = new Vector2(UIExtendControls.kThinHeight, UIExtendControls.kThinHeight);

            RectTransform checkmarkRect = checkmark.GetComponent<RectTransform>();
            checkmarkRect.anchorMin = new Vector2(0.5f, 0.5f);
            checkmarkRect.anchorMax = new Vector2(0.5f, 0.5f);
            checkmarkRect.anchoredPosition = Vector2.zero;
            checkmarkRect.sizeDelta = new Vector2(20f, 20f);

            RectTransform labelRect = childLabel.GetComponent<RectTransform>();
            labelRect.anchorMin     = new Vector2(0f, 0f);
            labelRect.anchorMax     = new Vector2(1f, 1f);
            labelRect.offsetMin     = new Vector2(23f, 1f);
            labelRect.offsetMax     = new Vector2(-5f, -2f);

            return toggleRoot;
        }

        // Input Field
        /*
        public static GameObject CreateInputField(UIExtendControls.Resources resources)
        {
            GameObject root = CreateUIElementRoot("InputField", s_ThickElementSize);

            GameObject childPlaceholder = CreateUIObject("Placeholder", root);
            GameObject childText = CreateUIObject("Text", root);

            Image image = root.AddComponent<Image>();
            image.sprite = resources.inputField;
            image.type = Image.Type.Sliced;
            image.color = UIExtendControls.s_DefaultSelectableColor;

            InputField inputField = root.AddComponent<InputField>();
            UIExtendControls.SetDefaultColorTransitionValues(inputField);

            Text text = childText.AddComponent<Text>();
            text.text = "";
            text.supportRichText = false;
            UIExtendControls.SetDefaultTextValues(text);

            Text placeholder = childPlaceholder.AddComponent<Text>();
            placeholder.text = "Enter text...";
            placeholder.fontStyle = FontStyle.Italic;
            // Make placeholder color half as opaque as normal text color.
            Color placeholderColor = text.color;
            placeholderColor.a *= 0.5f;
            placeholder.color = placeholderColor;

            RectTransform textRectTransform = childText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;
            textRectTransform.offsetMin = new Vector2(10, 6);
            textRectTransform.offsetMax = new Vector2(-10, -7);

            RectTransform placeholderRectTransform = childPlaceholder.GetComponent<RectTransform>();
            placeholderRectTransform.anchorMin = Vector2.zero;
            placeholderRectTransform.anchorMax = Vector2.one;
            placeholderRectTransform.sizeDelta = Vector2.zero;
            placeholderRectTransform.offsetMin = new Vector2(10, 6);
            placeholderRectTransform.offsetMax = new Vector2(-10, -7);

            inputField.textComponent = text;
            inputField.placeholder = placeholder;

            return root;
        }

        public static GameObject CreateDropdown(UIExtendControls.Resources resources)
        {
            GameObject root = CreateUIElementRoot("Dropdown", s_ThickElementSize);

            GameObject label = CreateUIObject("Label", root);
            GameObject arrow = CreateUIObject("Arrow", root);
            GameObject template = CreateUIObject("Template", root);
            GameObject viewport = CreateUIObject("Viewport", template);
            GameObject content = CreateUIObject("Content", viewport);
            GameObject item = CreateUIObject("Item", content);
            GameObject itemBackground = CreateUIObject("Item Background", item);
            GameObject itemCheckmark = CreateUIObject("Item Checkmark", item);
            GameObject itemLabel = CreateUIObject("Item Label", item);

            // Sub controls.

            GameObject scrollbar = CreateScrollbar(resources);
            scrollbar.name = "Scrollbar";
            SetParentAndAlign(scrollbar, template);

            Scrollbar scrollbarScrollbar = scrollbar.GetComponent<Scrollbar>();
            scrollbarScrollbar.SetDirection(Scrollbar.Direction.BottomToTop, true);

            RectTransform vScrollbarRT = scrollbar.GetComponent<RectTransform>();
            vScrollbarRT.anchorMin = Vector2.right;
            vScrollbarRT.anchorMax = Vector2.one;
            vScrollbarRT.pivot = Vector2.one;
            vScrollbarRT.sizeDelta = new Vector2(vScrollbarRT.sizeDelta.x, 0);

            // Setup item UI components.

            StringTableText itemLabelText = childText.AddComponent<StringTableText>();
            UIExtendControls.SetDefaultTextValues(itemLabelText);
            itemLabelText.alignment = TextAnchor.MiddleLeft;

            Image itemBackgroundImage = itemBackground.AddComponent<Image>();
            itemBackgroundImage.color = new Color32(245, 245, 245, 255);

            Image itemCheckmarkImage = itemCheckmark.AddComponent<Image>();
            itemCheckmarkImage.sprite = resources.checkmark;

            Toggle itemToggle = item.AddComponent<Toggle>();
            itemToggle.targetGraphic = itemBackgroundImage;
            itemToggle.graphic = itemCheckmarkImage;
            itemToggle.isOn = true;

            // Setup template UI components.

            Image templateImage = template.AddComponent<Image>();
            templateImage.sprite = resources.standard;
            templateImage.type = Image.Type.Sliced;

            ScrollRect templateScrollRect = template.AddComponent<ScrollRect>();
            templateScrollRect.content = (RectTransform)content.transform;
            templateScrollRect.viewport = (RectTransform)viewport.transform;
            templateScrollRect.horizontal = false;
            templateScrollRect.movementType = ScrollRect.MovementType.Clamped;
            templateScrollRect.verticalScrollbar = scrollbarScrollbar;
            templateScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            templateScrollRect.verticalScrollbarSpacing = -3;

            Mask scrollRectMask = viewport.AddComponent<Mask>();
            scrollRectMask.showMaskGraphic = false;

            Image viewportImage = viewport.AddComponent<Image>();
            viewportImage.sprite = resources.mask;
            viewportImage.type = Image.Type.Sliced;

            // Setup dropdown UI components.

            StringTableText labelText = childText.AddComponent<StringTableText>();
            UIExtendControls.SetDefaultTextValues(labelText);
            labelText.text = "Option A";
            labelText.alignment = TextAnchor.MiddleLeft;

            Image arrowImage = arrow.AddComponent<Image>();
            arrowImage.sprite = resources.dropdown;

            Image backgroundImage = root.AddComponent<Image>();
            backgroundImage.sprite = resources.standard;
            backgroundImage.color = UIExtendControls.s_DefaultSelectableColor;
            backgroundImage.type = Image.Type.Sliced;

            Dropdown dropdown = root.AddComponent<Dropdown>();
            dropdown.targetGraphic = backgroundImage;
            UIExtendControls.SetDefaultColorTransitionValues(dropdown);
            dropdown.template = template.GetComponent<RectTransform>();
            dropdown.captionText = labelText;
            dropdown.itemText = itemLabelText;

            // Setting default Item list.
            itemLabelText.text = "Option A";
            dropdown.options.Add(new Dropdown.OptionData {text = "Option A"});
            dropdown.options.Add(new Dropdown.OptionData {text = "Option B"});
            dropdown.options.Add(new Dropdown.OptionData {text = "Option C"});

            // Set up RectTransforms.

            RectTransform labelRT = label.GetComponent<RectTransform>();
            labelRT.anchorMin           = Vector2.zero;
            labelRT.anchorMax           = Vector2.one;
            labelRT.offsetMin           = new Vector2(10, 6);
            labelRT.offsetMax           = new Vector2(-25, -7);

            RectTransform arrowRT = arrow.GetComponent<RectTransform>();
            arrowRT.anchorMin           = new Vector2(1, 0.5f);
            arrowRT.anchorMax           = new Vector2(1, 0.5f);
            arrowRT.sizeDelta           = new Vector2(20, 20);
            arrowRT.anchoredPosition    = new Vector2(-15, 0);

            RectTransform templateRT = template.GetComponent<RectTransform>();
            templateRT.anchorMin        = new Vector2(0, 0);
            templateRT.anchorMax        = new Vector2(1, 0);
            templateRT.pivot            = new Vector2(0.5f, 1);
            templateRT.anchoredPosition = new Vector2(0, 2);
            templateRT.sizeDelta        = new Vector2(0, 150);

            RectTransform viewportRT = viewport.GetComponent<RectTransform>();
            viewportRT.anchorMin        = new Vector2(0, 0);
            viewportRT.anchorMax        = new Vector2(1, 1);
            viewportRT.sizeDelta        = new Vector2(-18, 0);
            viewportRT.pivot            = new Vector2(0, 1);

            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin         = new Vector2(0f, 1);
            contentRT.anchorMax         = new Vector2(1f, 1);
            contentRT.pivot             = new Vector2(0.5f, 1);
            contentRT.anchoredPosition  = new Vector2(0, 0);
            contentRT.sizeDelta         = new Vector2(0, 28);

            RectTransform itemRT = item.GetComponent<RectTransform>();
            itemRT.anchorMin            = new Vector2(0, 0.5f);
            itemRT.anchorMax            = new Vector2(1, 0.5f);
            itemRT.sizeDelta            = new Vector2(0, 20);

            RectTransform itemBackgroundRT = itemBackground.GetComponent<RectTransform>();
            itemBackgroundRT.anchorMin  = Vector2.zero;
            itemBackgroundRT.anchorMax  = Vector2.one;
            itemBackgroundRT.sizeDelta  = Vector2.zero;

            RectTransform itemCheckmarkRT = itemCheckmark.GetComponent<RectTransform>();
            itemCheckmarkRT.anchorMin   = new Vector2(0, 0.5f);
            itemCheckmarkRT.anchorMax   = new Vector2(0, 0.5f);
            itemCheckmarkRT.sizeDelta   = new Vector2(20, 20);
            itemCheckmarkRT.anchoredPosition = new Vector2(10, 0);

            RectTransform itemLabelRT = itemLabel.GetComponent<RectTransform>();
            itemLabelRT.anchorMin       = Vector2.zero;
            itemLabelRT.anchorMax       = Vector2.one;
            itemLabelRT.offsetMin       = new Vector2(20, 1);
            itemLabelRT.offsetMax       = new Vector2(-10, -2);

            template.SetActive(false);

            return root;
        }
        */
    }
}

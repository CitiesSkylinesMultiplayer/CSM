using System.Collections.Generic;
using ColossalFramework.UI;
using UnityEngine;

namespace CSM.Helpers
{
    /// <summary>
    ///     Working with UI is a pain. This class aims to make life easy!
    /// </summary>
    public static class UiHelpers
    {
        public static UIButton CreateButton(this UIComponent uiComponent, string text, Vector2 position, int width = 340,
            int height = 60)
        {
            UIButton button = (UIButton)uiComponent.AddUIComponent(typeof(UIButton));
            button.position = position;
            button.width = width;
            button.height = height;
            button.text = text;
            button.atlas = GetAtlas("Ingame");
            button.normalBgSprite = "ButtonMenu";
            button.hoveredBgSprite = "ButtonMenuHovered";
            button.pressedBgSprite = "ButtonMenuPressed";
            button.textColor = new Color32(200, 200, 200, 255);
            button.disabledTextColor = new Color32(50, 50, 50, 255);
            button.hoveredTextColor = new Color32(255, 255, 255, 255);
            button.pressedTextColor = new Color32(255, 255, 255, 255);
            button.playAudioEvents = true;
            button.isEnabled = true;
            button.isVisible = true;

            return button;
        }

        public static UILabel CreateTitleLabel(this UIComponent uiComponent, string text, Vector2 position)
        {
            UILabel label = uiComponent.CreateLabel(text, position);
            label.textAlignment = UIHorizontalAlignment.Center;
            label.textScale = 1.3f;
            label.height = 60;
            label.opacity = 0.8f;

            return label;
        }

        public static UILabel CreateLabel(this UIComponent uiComponent, string text, Vector2 position, int width = 340,
            int height = 60)
        {
            UILabel label = (UILabel)uiComponent.AddUIComponent(typeof(UILabel));
            label.position = position;
            label.text = text;
            label.width = width;
            label.height = height;
            label.textScale = 1.1f;

            return label;
        }

        public static UITextField CreateTextField(this UIComponent uiComponent, string placeholderText,
            Vector2 position, int width = 340,
            int height = 43)
        {
            UITextField textField = (UITextField)uiComponent.AddUIComponent(typeof(UITextField));
            textField.atlas = GetAtlas("Ingame");
            textField.position = position;
            textField.textScale = 1.5f;
            textField.width = width;
            textField.height = height;
            textField.padding = new RectOffset(6, 6, 8, 8);
            textField.builtinKeyNavigation = true;
            textField.isInteractive = true;
            textField.readOnly = false;
            textField.horizontalAlignment = UIHorizontalAlignment.Center;
            textField.selectionSprite = "EmptySprite";
            textField.selectionBackgroundColor = new Color32(0, 172, 234, 255);
            textField.normalBgSprite = "TextFieldPanelHovered";
            textField.disabledBgSprite = "TextFieldPanelHovered";
            textField.textColor = new Color32(0, 0, 0, 255);
            textField.disabledTextColor = new Color32(80, 80, 80, 128);
            textField.color = new Color32(255, 255, 255, 255);
            textField.text = placeholderText;

            return textField;
        }

        public static UICheckBox CreateCheckBox(this UIComponent uiComponent, string text, Vector2 position)
        {
            UICheckBox checkBox = (UICheckBox)uiComponent.AddUIComponent(typeof(UICheckBox));

            checkBox.width = 300f;
            checkBox.height = 20f;
            checkBox.clipChildren = true;
            checkBox.position = position;

            UISprite sprite = checkBox.AddUIComponent<UISprite>();
            sprite.atlas = GetAtlas("Ingame");
            sprite.spriteName = "ToggleBase";
            sprite.size = new Vector2(16f, 16f);
            sprite.relativePosition = Vector3.zero;

            checkBox.checkedBoxObject = sprite.AddUIComponent<UISprite>();
            ((UISprite)checkBox.checkedBoxObject).atlas = GetAtlas("Ingame");
            ((UISprite)checkBox.checkedBoxObject).spriteName = "ToggleBaseFocused";
            checkBox.checkedBoxObject.size = new Vector2(16f, 16f);
            checkBox.checkedBoxObject.relativePosition = Vector3.zero;
            checkBox.checkedBoxObject.isInteractive = false;

            checkBox.label = checkBox.AddUIComponent<UILabel>();
            checkBox.label.text = text;
            checkBox.label.textScale = 0.9f;
            checkBox.label.relativePosition = new Vector3(22f, 2f);

            return checkBox;
        }

        public static void AddScrollbar(this UIComponent uiComponent, UIScrollablePanel scrollablePanel)
        {
            UIScrollbar scrollbar = uiComponent.AddUIComponent<UIScrollbar>();
            scrollbar.width = 20f;
            scrollbar.height = scrollablePanel.height;
            scrollbar.orientation = UIOrientation.Vertical;
            scrollbar.pivot = UIPivotPoint.TopLeft;
            scrollbar.position = scrollablePanel.position + new Vector3(scrollablePanel.width - 20, 0);
            scrollbar.minValue = 0;
            scrollbar.value = 0;
            scrollbar.incrementAmount = 50;
            scrollbar.name = "PanelScrollBar";

            // Add scrollbar background sprite component
            UISlicedSprite trackingSprite = scrollbar.AddUIComponent<UISlicedSprite>();
            trackingSprite.position = new Vector2(0, 0);
            trackingSprite.autoSize = true;
            trackingSprite.size = trackingSprite.parent.size;
            trackingSprite.fillDirection = UIFillDirection.Vertical;
            trackingSprite.spriteName = "ScrollbarTrack";
            trackingSprite.name = "PanelTrack";
            scrollbar.trackObject = trackingSprite;
            scrollbar.trackObject.height = scrollbar.height;

            // Add scrollbar thumb component
            UISlicedSprite trackingThumb = scrollbar.AddUIComponent<UISlicedSprite>();
            trackingThumb.position = new Vector2(0, 0);
            trackingThumb.fillDirection = UIFillDirection.Vertical;
            trackingThumb.autoSize = true;
            trackingThumb.width = trackingThumb.parent.width - 8;
            trackingThumb.spriteName = "ScrollbarThumb";
            trackingThumb.name = "PanelThumb";

            scrollbar.thumbObject = trackingThumb;
            scrollbar.isVisible = true;
            scrollbar.isEnabled = true;
            scrollablePanel.verticalScrollbar = scrollbar;
        }

        public static void Remove(this UIComponent uiComponent)
        {
            uiComponent.parent.RemoveUIComponent(uiComponent);
            Object.DestroyImmediate(uiComponent.gameObject);
        }

        // Sourced from : https://github.com/SamsamTS/CS-MoveIt/blob/master/MoveIt/GUI/UIUtils.cs
        // I found their code after I started this class, the below atlas feature is quite neat!

        private static Dictionary<string, UITextureAtlas> _atlases;

        public static UITextureAtlas GetAtlas(string name)
        {
            if (_atlases == null)
            {
                _atlases = new Dictionary<string, UITextureAtlas>();

                UITextureAtlas[] atlases = Resources.FindObjectsOfTypeAll(typeof(UITextureAtlas)) as UITextureAtlas[];
                foreach (UITextureAtlas atlas in atlases)
                {
                    if (!_atlases.ContainsKey(atlas.name))
                        _atlases.Add(atlas.name, atlas);
                }
            }

            return _atlases[name];
        }
    }
}

using System.Collections.Generic;
using ColossalFramework.UI;
using UnityEngine;

namespace Tango.Helpers
{
    /// <summary>
    /// Working with UI is a pain. This class aims to make life easy!
    /// </summary>
    public static class UiHelpers
    {
        public static UIButton CreateButton(this UIComponent uiComponent, string text, Vector2 position, int width = 340, 
            int height = 60)
        {
            var button = (UIButton)uiComponent.AddUIComponent(typeof(UIButton));
            button.position = position;
            button.width = width;
            button.height = height;
            button.text = text;
            button.atlas = GetAtlas("Ingame");
            button.normalBgSprite = "ButtonMenu";
            button.hoveredBgSprite = "ButtonMenuHovered";
            button.pressedBgSprite = "ButtonMenuPressed";
            button.textColor = new Color32(255, 51, 153, 150);
            button.disabledTextColor = new Color32(7, 7, 7, 200);
            button.hoveredTextColor = new Color32(255, 255, 255, 255);
            button.pressedTextColor = new Color32(204, 0, 0, 255);
            button.playAudioEvents = true;
            button.isEnabled = true;
            button.isVisible = true;

            return button;
        }

        public static UILabel CreateTitleLabel(this UIComponent uiComponent, string text, Vector2 position)
        {
            var label = uiComponent.CreateLabel(text, position);
            label.textAlignment = UIHorizontalAlignment.Center;
            label.textScale = 1.3f;
            label.height = 60;
            label.opacity = 0.8f;

            return label;
        }

        public static UILabel CreateLabel(this UIComponent uiComponent, string text, Vector2 position, int width = 340,
            int height = 60)
        {
            var label = (UILabel)uiComponent.AddUIComponent(typeof(UILabel));
            label.position = position;
            label.text = text;
            label.width = width;
            label.height = height;
            label.textScale = 1.1f;

            return label;
        }

        public static UITextField CreateTextField(this UIComponent uiComponent, string placeholderText,
            Vector2 position, int width = 340,
            int height = 40)
        {
            var textField = (UITextField)uiComponent.AddUIComponent(typeof(UITextField));
            textField.atlas = GetAtlas("Ingame");
            textField.position = position;
            textField.textScale = 1.5f;
            textField.width = width;
            textField.height = height;
            textField.padding = new RectOffset(6, 6, 2, 2);
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

        // Sourced from : https://github.com/SamsamTS/CS-MoveIt/blob/master/MoveIt/UIUtils.cs
        // I found his code after I started this class, the below atlas feature is quite neat!

        private static Dictionary<string, UITextureAtlas> _atlases;
        public static UITextureAtlas GetAtlas(string name)
        {
            if (_atlases == null)
            {
                _atlases = new Dictionary<string, UITextureAtlas>();

                UITextureAtlas[] atlases = Resources.FindObjectsOfTypeAll(typeof(UITextureAtlas)) as UITextureAtlas[];
                for (int i = 0; i < atlases.Length; i++)
                {
                    if (!_atlases.ContainsKey(atlases[i].name))
                        _atlases.Add(atlases[i].name, atlases[i]);
                }
            }

            return _atlases[name];
        }
    }
}

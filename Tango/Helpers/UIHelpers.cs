using ColossalFramework.UI;
using UnityEngine;

namespace Tango.Helpers
{
    /// <summary>
    /// Working with UI is a pain. This class aims to make life easy!
    /// </summary>
    public static class UiHelpers
    {
        /// <summary>
        /// Create a default style button
        /// </summary>
        /// <param name="uiComponent"></param>
        /// <param name="text"></param>
        /// <param name="position"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static UIButton CreateButton(this UIComponent uiComponent, string text, Vector2 position, int width = 340, int height = 60)
        {
            var button = (UIButton)uiComponent.AddUIComponent(typeof(UIButton));
            button.position = position;
            button.width = width;
            button.height = height;
            button.text = text;
            button.normalBgSprite = "ButtonMenu";
            button.disabledBgSprite = "ButtonMenuDisabled";
            button.hoveredBgSprite = "ButtonMenuHovered";
            button.focusedBgSprite = "ButtonMenu";
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
    }
}

using ColossalFramework.UI;
using UnityEngine;

namespace CSM.Panels
{
    public static class PanelManager
    {
        private static UIView _uiView;

        public static T GetPanel<T>() where T : UIComponent
        {
            if (!_uiView)
                _uiView = UIView.GetAView();

            if (!_uiView)
                return null; // No ui view available yet

            string name = typeof(T).Name;
            return _uiView.FindUIComponent<T>(name);
        }

        public static T TogglePanel<T>() where T : UIComponent
        {
            return ShowPanel<T>(true);
        }

        public static T ShowPanel<T>() where T : UIComponent
        {
            return ShowPanel<T>(false);
        }

        public static void HidePanel<T>() where T : UIComponent
        {
            T panel = GetPanel<T>();
            if (panel)
            {
                panel.isVisible = false;
            }
        }

        private static T ShowPanel<T>(bool toggle) where T : UIComponent
        {
            T panel = GetPanel<T>();

            if (panel)
            {
                if (toggle)
                    panel.isVisible = !panel.isVisible;
                else
                    panel.isVisible = true;
            }
            else if (_uiView != null)
            {
                panel = (T)_uiView.AddUIComponent(typeof(T));
                panel.name = typeof(T).Name;
            }
            else
            {
                return null;
            }
            panel.Focus();

            return panel;
        }

        public static Vector3 GetCenterPosition(UIPanel panel)
        {
            UIView view = panel.GetUIView();
            float actualWidth = view.GetScreenResolution().x;
            float actualHeight = view.GetScreenResolution().y;
            return new Vector3(actualWidth  / 2.0f - panel.width  / 2.0f,
                               actualHeight / 2.0f - panel.height / 2.0f);
        }
    }
}

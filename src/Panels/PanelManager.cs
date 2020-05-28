using ColossalFramework.UI;

namespace CSM.Panels
{
    public static class PanelManager
    {
        private static UIView _uiView;

        public static T TogglePanel<T>() where T : UIComponent
        {
            return ShowPanel<T>(true);
        }

        public static T ShowPanel<T>() where T : UIComponent
        {
            return ShowPanel<T>(false);
        }

        private static T ShowPanel<T>(bool toggle) where T: UIComponent
        {
            if (!_uiView)
                _uiView = UIView.GetAView();

            string name = typeof(T).Name;
            T panel = _uiView.FindUIComponent<T>(name);

            if (panel)
            {
                if (toggle)
                    panel.isVisible = !panel.isVisible;
                else
                    panel.isVisible = true;
            }
            else
            {
                panel = (T) _uiView.AddUIComponent(typeof(T));
                panel.name = name;
            }
            panel.Focus();

            return panel;
        }
    }
}

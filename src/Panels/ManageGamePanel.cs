using ColossalFramework.UI;
using CSM.Helpers;
using CSM.Networking;
using UnityEngine;

namespace CSM.Panels
{
    public class ManageGamePanel : UIPanel
    {
        private UITextField _portField;

        private UIButton _closeButton;
        private UIButton _listButton;

        public override void Start()
        {
            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            name = "MPManageGamePanel";
            color = new Color32(110, 110, 110, 250);

            // Grab the view for calculating width and height of game
            var view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(view.fixedWidth / 2.0f - 180.0f, view.fixedHeight / 2.0f - 240.0f);

            width = 360;
            height = 480;

            // Title Label
            this.CreateTitleLabel("Manage Server", new Vector2(90, -20));

            // IP Address Label
            this.CreateLabel("Port:", new Vector2(10, -75));

            // Port field
            _portField = this.CreateTextField(MultiplayerManager.Instance.CurrentServer.Config.Port.ToString(), new Vector2(10, -100));
            _portField.isEnabled = false;

            _listButton = this.CreateButton("View Players", new Vector2(10, -150));
            _listButton.eventClick += (component, param) =>
            {
                var panel = view.FindUIComponent<PlayerListPanel>("MPPlayerListPanel");

                if (panel != null)
                {
                    panel.isVisible = true;
                }
                else
                {
                    panel = (PlayerListPanel)view.AddUIComponent(typeof(PlayerListPanel));
                }

                panel.Focus();
                isVisible = false;
            };

            // Close this dialog
            _closeButton = this.CreateButton("Close", new Vector2(10, -410));
            _closeButton.eventClick += (component, param) =>
            {
                isVisible = false;
            };

            eventVisibilityChanged += (component, value) =>
            {
                _portField.text = MultiplayerManager.Instance.CurrentServer.Config.Port.ToString();
            };
        }
    }
}
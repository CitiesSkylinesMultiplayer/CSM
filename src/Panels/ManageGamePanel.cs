using ColossalFramework.UI;
using CSM.Helpers;
using UnityEngine;

namespace CSM.Panels
{
    public class ManageGamePanel : UIPanel
    {
        private UITextField _portField;
        private int _port = 4230;

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
            relativePosition = new Vector3(view.fixedWidth / 2.0f - 180.0f, view.fixedHeight / 2.0f - 200.0f);

            width = 360;
            height = 480;

            // Title Label
            this.CreateTitleLabel("Manage server", new Vector2(90, -20));

            // IP Address Label
            this.CreateLabel("Port:", new Vector2(10, -75));

            // Port field
            _portField = this.CreateTextField("4230", new Vector2(10, -100));
            _portField.isEnabled = false;

            _listButton = this.CreateButton("Player list", new Vector2(10, -150));
            _listButton.eventClick += (component, param) => {

                var panel = view.FindUIComponent<PlayerListPanel>("MPPlayerListPanel");

                if (panel != null) {
                    panel.isVisible = true;
                } else {
                    panel = (PlayerListPanel) view.AddUIComponent(typeof(PlayerListPanel));
                }

                panel.Focus();
                isVisible = false;
            };

            // Close this dialog
            _closeButton = this.CreateButton("Cancel", new Vector2(10, -410));
            _closeButton.eventClick += (component, param) => {
                isVisible = false;
            };

            eventVisibilityChanged += (component, value) => {
                _portField.text = "" + _port;
            };
        }

        public void SetPort(int port)
        {
            _port = port;
        }
    }
}

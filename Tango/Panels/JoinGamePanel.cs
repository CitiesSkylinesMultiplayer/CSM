using ColossalFramework.UI;
using Tango.Helpers;
using UnityEngine;

namespace Tango.Panels
{
    public class JoinGamePanel : UIPanel
    {
        private UILabel _title;

        private UILabel _ipAddressLabel;
        private UITextField _ipAddressField;

        private UILabel _portLabel;
        private UITextField _portField;

        private UIButton _connectButton;
        private UIButton _closeButton;

        public override void Start()
        {
            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            name = "MPJoinGamePanel";
            color = new Color32(110, 110, 110, 250);

            // Grab the view for caculating width and height of game
            var view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(view.fixedWidth / 2.0f - 180.0f, view.fixedHeight / 2.0f - 200.0f);

            width = 360;
            height = 400;

            // Title Label
            _title = (UILabel)AddUIComponent(typeof(UILabel));
            _title.position = new Vector3(0, 0, 0);
            _title.text = "Connect to Server";
            _title.textAlignment = UIHorizontalAlignment.Center;
            _title.width = 340;
            _title.textScale = 1.2f;
            _title.opacity = 0.8f;
            _title.height = 60;
            _title.position = new Vector3(80, -20, 0);

            // IP Address Label
            _ipAddressLabel = (UILabel) AddUIComponent(typeof(UILabel));
            _ipAddressLabel.text = "IP Address:";
            _ipAddressLabel.textScale = 1.1f;
            _ipAddressLabel.position = new Vector3(20, -80, 0);
     
            // IP Address field
            _ipAddressField = (UITextField) AddUIComponent(typeof(UITextField));
            _ipAddressField.position = new Vector3(20, -100, 0);
            _ipAddressField.width = 330;
            _ipAddressField.height = 40;
            _ipAddressField.text = "localhost";
            _ipAddressField.isEnabled = true;
            _ipAddressField.normalBgSprite = "TextFieldPanel";
            _ipAddressField.selectionSprite = "EmptySprite";
            _ipAddressField.builtinKeyNavigation = true;
            _ipAddressField.isInteractive = true;
            _ipAddressField.readOnly = false;
            _ipAddressField.maxLength = 48;
        
            // Port Label
            _portLabel = (UILabel)AddUIComponent(typeof(UILabel));
            _portLabel.text = "Port";
            _portLabel.textScale = 1.1f;
            _portLabel.position = new Vector3(20, -160, 0);

            // Port field
            _portField = (UITextField)AddUIComponent(typeof(UITextField));
            _portField.position = new Vector3(20, -200, 0);
            _portField.width = 330;
            _portField.height = 40;
            _portField.text = "4230";
            _portField.isEnabled = true;
            _portField.normalBgSprite = "TextFieldPanel";
            _portField.selectionSprite = "EmptySprite";
            _portField.builtinKeyNavigation = true;
            _portField.isInteractive = true;
            _portField.readOnly = false;
            _portField.maxLength = 10;

            // Connect to Server Button
            _connectButton = this.CreateButton("Connect", new Vector3(20, -250, 0), 330);

            // Close this dialog
            _closeButton = this.CreateButton("Close", new Vector3(20, -250, 0), 330);
            _closeButton.eventClick += (component, param) =>
            {
                isVisible = false;
                Hide();
            };
        }
    }
}

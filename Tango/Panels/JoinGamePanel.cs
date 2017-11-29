using ColossalFramework.UI;
using UnityEngine;

namespace Tango.Panels
{
    public class JoinGamePanel : UIPanel
    {
        private UILabel _title;

        private UITextField _ipAddressField;
        private UITextField _portField;

        

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

            _title = (UILabel)AddUIComponent(typeof(UILabel));
            _title.position = new Vector3(0, 0, 0);
            _title.text = "Connect to Server";
            _title.textAlignment = UIHorizontalAlignment.Center;
            _title.width = 340;
            _title.textScale = 1.2f;
            _title.opacity = 0.8f;
            _title.height = 60;
            _title.position = new Vector3(80, -20, 0);

            _ipAddressField = (UITextField) AddUIComponent(typeof(UITextField));
            _ipAddressField.position = new Vector3(10, -60, 0);
            _ipAddressField.width = 340;
            _ipAddressField.height = 60;
            _ipAddressField.text = "IP Address";

            _portField = (UITextField)AddUIComponent(typeof(UITextField));
            _portField.position = new Vector3(10, -130, 0);
            _portField.width = 340;
            _portField.height = 60;
            _portField.text = "Port";
        }
    }
}

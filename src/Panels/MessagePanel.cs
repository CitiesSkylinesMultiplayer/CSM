using ColossalFramework.UI;
using CSM.Helpers;
using CSM.Util;
using System.Linq;
using UnityEngine;

namespace CSM.Panels
{
    public class MessagePanel : UIPanel
    {
        private UILabel _titleLabel;
        private UILabel _messageLabel;
        private string _title;
        private string _message;

        private UIButton _closeButton;

        public override void Start()
        {
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            color = new Color32(110, 110, 110, 255);

            // Grab the view for calculating width and height of game
            UIView view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(view.fixedWidth / 2.0f - 225.0f, view.fixedHeight / 2.0f - 200.0f);

            width = 450;
            height = 500;

            // Title Label
            _titleLabel = this.CreateTitleLabel(_title, new Vector2(150, -20));

            // Mismatch message content
            _messageLabel = AddUIComponent<UILabel>();
            _messageLabel.text = _message;
            _messageLabel.position = new Vector2(10, -70);

            // Close button
            _closeButton = this.CreateButton("Close", new Vector2(60, -410));

            _closeButton.eventClicked += (component, param) => Hide();
        }

        public void DisplayContentWarning()
        {
            _title = "Warning";
            if (_titleLabel)
                _titleLabel.text = _title;

            _message = "Playing with other mods or DLCs is\n" +
                       "currently not officially supported.\n\n" +
                       "Try to disable them if you encounter\n" +
                       "any issues.";

            if (_messageLabel)
                _messageLabel.text = _message;

            Show(true);
        }

        public void DisplayDlcMessage(DLCHelper.DLCComparison compare)
        {
            _title = "DLC Mismatch";
            if (_titleLabel)
                _titleLabel.text = _title;

            DLCPanelNew dlcPanel = FindObjectOfType<DLCPanelNew>();

            string message = "Your DLCs don't match with the server's DLCs\n\n";
            if (compare.ClientMissing != SteamHelper.DLC_BitMask.None)
            {
                message += "You are missing the following DLCs:\n";
                message += string.Join("\n", compare.ClientMissing.DLCs().Select(dlc => dlcPanel.FindLocalizedDLCName(dlc)).ToArray());
                message += "\n\n";
            }
            if (compare.ServerMissing != SteamHelper.DLC_BitMask.None)
            {
                message += "The server doesn't have the following DLCs:\n";
                message += string.Join("\n", compare.ServerMissing.DLCs().Select(dlc => dlcPanel.FindLocalizedDLCName(dlc)).ToArray());
            }

            message += "\n\nDLCs can be enabled/disabled via checkbox in Steam.";

            _message = message;

            if (_messageLabel)
                _messageLabel.text = message;

            Show(true);

            Log.Info("DLCs don't match:\n" + message);
        }
    }
}

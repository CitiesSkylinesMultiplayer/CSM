using System;
using System.Globalization;
using ColossalFramework.UI;
using CSM.Helpers;
using CSM.Util;
using System.Linq;
using System.Reflection;
using CSM.Networking;
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

            // Center this window in the game
            relativePosition = new Vector3(Screen.width / 2.0f - 225.0f, Screen.height / 2.0f - 200.0f);

            width = 450;
            height = 500;

            // Title Label
            _titleLabel = this.CreateTitleLabel("", new Vector2(150, -20));
            SetTitle(_title);

            // Mismatch message content
            _messageLabel = AddUIComponent<UILabel>();
            _messageLabel.text = _message;
            _messageLabel.position = new Vector2(10, -70);

            // Close button
            _closeButton = this.CreateButton("Close", new Vector2(60, -410));

            _closeButton.eventClicked += (component, param) => Hide();
        }

        private void SetTitle(string title)
        {
            _title = title;

            if (_titleLabel)
            {
                _titleLabel.text = title;
                Vector3 pos = _titleLabel.position;
                pos.x = (width - _titleLabel.width) / 2;
                _titleLabel.position = pos;
            }
        }

        private void SetMessage(string message)
        {
            _message = message;

            if (_messageLabel)
                _messageLabel.text = message;
        }

        public void DisplayContentWarning()
        {
            SetTitle("Warning");

            const string message = "Playing with other mods or DLCs is\n" +
                       "currently not officially supported.\n\n" +
                       "Try to disable them if you encounter\n" +
                       "any issues.";

            SetMessage(message);

            Show(true);
        }

        private string GetDlcName(DLCPanelNew panel, SteamHelper.DLC dlc)
        {
            string dlcName = panel.FindLocalizedDLCName(dlc);
            if (string.IsNullOrEmpty(dlcName))
            {
                // Default to enum item name
                dlcName = dlc.ToString();
            }

            return dlcName;
        }

        public void DisplayDlcMessage(DLCHelper.DLCComparison compare)
        {
            SetTitle("DLC Mismatch");

            DLCPanelNew dlcPanel = FindObjectOfType<DLCPanelNew>();

            string message = "Your DLCs don't match with the server's DLCs\n\n";
            if (compare.ClientMissing != SteamHelper.DLC_BitMask.None)
            {
                message += "You are missing the following DLCs:\n";
                message += string.Join("\n", compare.ClientMissing.DLCs().Select(dlc => GetDlcName(dlcPanel, dlc)).ToArray());
                message += "\n\n";
            }
            if (compare.ServerMissing != SteamHelper.DLC_BitMask.None)
            {
                message += "The server doesn't have the following DLCs:\n";
                message += string.Join("\n", compare.ServerMissing.DLCs().Select(dlc => GetDlcName(dlcPanel, dlc)).ToArray());
            }

            message += "\n\nDLCs can be enabled/disabled via checkbox in Steam.";

            SetMessage(message);

            Show(true);

            Log.Info("DLCs don't match:\n" + message);
        }

        public void DisplayReleaseNotes()
        {
            SetTitle("Release Notes - Multiplayer (CSM)");

            Version version = Assembly.GetAssembly(typeof(CSM)).GetName().Version;

            string message = $"Version {version.Major}.{version.Minor}\n" +
                             $"Last Update: April 4, 2021\n\n" +
                             "- UI Changes:\n" +
                             "  - The Chirper is now used as the chat\n" +
                             "    (The old chat can still be enabled in the settings)\n" +
                             "  - The multiplayer menu can now be found\n" +
                             "    in the pause menu\n" +
                             "  - Added this release notes panel\n\n" +
                             "- Fixes:\n" +
                             "  - Tried to fix issue with not being able to change\n" +
                             "    the speed or pause state (Please tell us on Discord\n" +
                             "    if the problems are now solved for you!).\n";

            SetMessage(message);

            Show(true);
        }
    }
}

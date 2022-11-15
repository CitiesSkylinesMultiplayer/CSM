using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ColossalFramework.UI;
using CSM.API;
using CSM.Helpers;
using UnityEngine;

namespace CSM.Panels
{
    public class MessagePanel : UIPanel
    {
        private UILabel _titleLabel;
        private UILabel _messageLabel;
        private string _title;
        private string _message;

        private UIButton _closeButton, _githubButton;
        private bool _githubShown = false;

        public override void Start()
        {
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            color = new Color32(110, 110, 110, 255);

            width = 450;
            height = 500;
            relativePosition = PanelManager.GetCenterPosition(this);

            // Title Label
            _titleLabel = this.CreateTitleLabel("", new Vector2(150, -20));
            SetTitle(_title);

            UIScrollablePanel messagePanel = AddUIComponent<UIScrollablePanel>();
            messagePanel.name = "innerMessagePanel";
            messagePanel.width = 430;
            messagePanel.height = 310;
            messagePanel.clipChildren = true;
            messagePanel.position = new Vector2(10, -70);

            // Mismatch message content
            _messageLabel = messagePanel.AddUIComponent<UILabel>();
            _messageLabel.text = _message;
            _messageLabel.position = new Vector2(10, 0);

            this.AddScrollbar(messagePanel);

            // Github button
            _githubButton = this.CreateButton("Open GitHub", new Vector2(60, -340));
            _githubButton.eventClicked += (c, p) =>
            {
                Process.Start("https://github.com/CitiesSkylinesMultiplayer/CSM/releases");
            };
            _githubButton.isVisible = _githubShown;

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

            if (_githubButton)
                _githubButton.Hide();
        }

        public void DisplayContentWarning()
        {
            SetTitle("Warning");

            const string message = "Playing with DLCs is\n" +
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
            if (compare.ClientMissingExpansions != SteamHelper.ExpansionBitMask.None || compare.ClientMissingModderPack != SteamHelper.ModderPackBitMask.None)
            {
                message += "You are missing the following DLCs:\n";
                message += string.Join("\n", SteamHelper.DLCs(compare.ClientMissingExpansions, compare.ClientMissingModderPack, SteamHelper.RadioBitMask.None).Select(dlc => GetDlcName(dlcPanel, dlc)).ToArray());
                message += "\n\n";
            }
            if (compare.ServerMissingExpansions != SteamHelper.ExpansionBitMask.None || compare.ServerMissingModderPack != SteamHelper.ModderPackBitMask.None)
            {
                message += "The server doesn't have the following DLCs:\n";
                message += string.Join("\n", SteamHelper.DLCs(compare.ServerMissingExpansions, compare.ServerMissingModderPack, SteamHelper.RadioBitMask.None).Select(dlc => GetDlcName(dlcPanel, dlc)).ToArray());
            }

            message += "\n\nDLCs can be enabled/disabled via checkbox in Steam.";

            SetMessage(message);

            Show(true);

            Log.Info("DLCs don't match:\n" + message);
        }

        public void DisplayModsMessage(IEnumerable<string> serverNotClient, IEnumerable<string> clientNotServer)
        {
            SetTitle("Mod Mismatch");

            string message = "Your installed mods don't match with the server's\n\n";
            string[] notClient = serverNotClient.ToArray();
            if (notClient.Length > 0)
            {
                message += "You are missing the following mods/assets:\n";
                message += "- " + string.Join("\n- ", notClient);
                message += "\n\n";
            }

            string[] notServer = clientNotServer.ToArray();
            if (notServer.Length > 0)
            {
                message += "The server doesn't have the following mods/assets:\n";
                message += "- " + string.Join("\n- ", notServer);
            }

            message += "\n\nYou can enable/disable mods from the content\n manager in the main menu.";

            SetMessage(message);

            Show(true);

            Log.Info("Mods don't match:\n" + message);
        }

        public void DisplayReleaseNotes()
        {
            SetTitle("Release Notes - Multiplayer (CSM)");

            Version version = Assembly.GetAssembly(typeof(CSM)).GetName().Version;

            string message = $"Version {version.Major}.{version.Minor}\n" +
                             "Last Update: November 15th, 2022\n\n" +
                             "- Features:\n" +
                             "  - Support Roads and Vehicles Update\n\n" +
                             " - Fixes:\n" +
                             "  - Ignore order of mods for compatibility\n";
            SetMessage(message);

            Show(true);
        }

        public void DisplayUpdateAvailable(Version current, Version latest)
        {
            SetTitle("CSM Update available!");

            string message = "A new update of the Cities: Skylines Multiplayer\n" +
                             "mod is available!\n\n" +
                             $"Current Version: {current.Major}.{current.Minor}\n" +
                             $"Latest Version: {latest.Major}.{latest.Minor}\n\n" +
                             "When using the Steam Workshop, it should be\n" +
                             "installed automatically (you may need to restart\n" +
                             "your game). Otherwise you can download the\n" +
                             "update from GitHub:\n\n" +
                             "https://github.com/CitiesSkylinesMultiplayer/\n" +
                             "CSM/releases";
            SetMessage(message);

            _githubShown = true;
            if (_githubButton)
                _githubButton.Show();

            Show(true);
        }

        public void DisplayNoUpdateAvailable()
        {
            SetTitle("CSM is up to date");

            string message = "There is no update for the Cities: Skylines\n" +
                             "Multiplayer mod available.";
            SetMessage(message);

            Show(true);
        }
    }
}

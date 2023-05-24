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

        public void DisplayInvalidApiServer()
        {
            SetTitle("Invalid API Server");

            const string message = "The given API server could not be reached.\n" +
                                   "The URL was not changed.";

            SetMessage(message);

            Show(true);
        }

        private string GetDlcName(DLCList list, SteamHelper.DLC dlc)
        {
            string dlcName = list.FindLocalizedDLCName(dlc);
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

            DLCList dlcPanel = FindObjectOfType<DLCList>();

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
                             "Last Update: May 25th, 2023\n\n" +
                             "- Fixes:\n" +
                             "  - Fix problems of new hotels update\n" +
                             "  - Fix exception reported by a few members on Discord";
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

            const string message = "There is no update for the Cities: Skylines\n" +
                                   "Multiplayer mod available.";
            SetMessage(message);

            Show(true);
        }

        public void DisplayTroubleshooting(bool isHost, int port=4230, bool hasVpn=false)
        {
            SetTitle("Troubleshooting");

            string message;

            if (isHost)
            {
                message = "When the port is not reachable, this can have\n" +
                          "various reasons. You can try the following steps:\n\n" +
                          "   - Forward the CSM port: You need to\n" +
                          "     forward the port (" + port + " UDP)\n" +
                          "     in your router. How this works depends on\n" +
                          "     the router or internet provider.\n" +
                          "   -> If it doesn't work yet, you might\n" +
                          "     need to allow the port through the local\n" +
                          "     Firewall (e.g. Windows Defender Firewall).\n" +
                          "     You can find more info on the Internet.\n" +
                          "   -> You can always check again if the\n" +
                          "     port is working in the \"Manage Server\" menu.";
                if (hasVpn)
                {
                    message += "\n\n   - Since Hamachi seems to be running,\n" +
                               "     players can also connect using your\n" +
                               "     Hamachi IP.\n" +
                               "     Also check the Firewall in this case.";
                }
                else
                {
                    message += "\n\n   - Alternatively, you can use a VPN solution\n" +
                               "     like Hamachi or ZeroTier.\n" +
                               "     Follow the instructions of the respective\n" +
                               "     tool. Then players can connect using the\n" +
                               "     displayed IP address of the tool.\n" +
                               "     You can then ignore any error messages\n" +
                               "     regarding port forwarding.";
                }
            }
            else
            {
                message = "When the \"Failed to connect\" message appears,\n" +
                          "this can have various reasons. The hosting player\n" +
                          "can try the following steps:\n\n" +
                          "- Check if the port is reachable in the\n" +
                          "  \"Manage Server\" menu\n" +
                          "  - If yes, make sure you connect using the IP\n" +
                          "    shown as \"External IP\" in that menu\n\n" +
                          "- If not, you have two possibilities:\n" +
                          "  1. Forward the CSM port: The hosting player\n" +
                          "     needs to forward the port (" + port + " UDP)\n" +
                          "     in their router. How this works depends on\n" +
                          "     the router or internet provider.\n" +
                          "   -> If it doesn't work yet, the host might\n" +
                          "     need to allow the port through the local\n" +
                          "     Firewall (e.g. Windows Defender Firewall).\n" +
                          "     You can find more info on the Internet.\n" +
                          "   -> The host can always check again if the\n" +
                          "     port is working in the \"Manage Server\" menu.\n\n" +
                          "  2. Use a VPN solution like Hamachi or ZeroTier.\n" +
                          "     Follow the instructions of the respective\n" +
                          "     tool. Then you can connect using the\n" +
                          "     displayed IP address of the tool.";
            }

            SetMessage(message);

            Show(true);
        }

        public void DisplayJoiningNotAllowed()
        {
            SetTitle("Invalid State");

            const string message = "You need to be in the main menu to join\n" +
                                   "a server through the friend list.";

            SetMessage(message);

            Show(true);
        }
    }
}

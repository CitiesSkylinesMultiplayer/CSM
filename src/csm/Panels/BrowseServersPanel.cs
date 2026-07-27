using System;
using System.Collections.Generic;
using System.Net;
using ColossalFramework.PlatformServices;
using ColossalFramework.UI;
using CSM.API;
using CSM.GS.Commands;
using CSM.GS.Commands.Data.ApiServer;
using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Config;
using LiteNetLib;
using UnityEngine;

namespace CSM.Panels
{
    public class BrowseServersPanel : UIPanel
    {
        private static readonly TimeSpan RequestInterval = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan ResponseTimeout = TimeSpan.FromSeconds(5);

        private UIScrollablePanel _scrollablePanel;
        private UILabel _statusLabel;
        private UIButton _refreshButton;
        private UIButton _closeButton;

        private readonly List<UILabel> _rowLabels = new List<UILabel>();
        private readonly List<UIButton> _joinButtons = new List<UIButton>();

        private LiteNetLib.NetManager _netManager;
        private DateTime _lastRequestSent = DateTime.MinValue;
        private bool _awaitingResponse;

        public override void Start()
        {
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            color = new Color32(110, 110, 110, 255);

            width = 450;
            height = 500;
            relativePosition = PanelManager.GetCenterPosition(this);

            this.CreateTitleLabel("Public Servers", new Vector2(140, -20));

            _statusLabel = this.CreateLabel("Loading...", new Vector2(10, -60));
            _statusLabel.textAlignment = UIHorizontalAlignment.Center;
            _statusLabel.width = 430;

            _scrollablePanel = AddUIComponent<UIScrollablePanel>();
            _scrollablePanel.width = 410;
            _scrollablePanel.height = 330;
            _scrollablePanel.position = new Vector2(10, -85);
            _scrollablePanel.clipChildren = true;
            _scrollablePanel.name = "BrowseServersScrollablePanel";

            this.AddScrollbar(_scrollablePanel);

            _refreshButton = this.CreateButton("Refresh", new Vector2(10, -430), 200);
            _refreshButton.eventClick += (component, param) => SendServerListRequest();

            _closeButton = this.CreateButton("Close", new Vector2(220, -430), 200);
            _closeButton.eventClick += (component, param) =>
            {
                isVisible = false;
            };

            SetupNetworking();
        }

        public override void Update()
        {
            if (!isVisible)
                return;
            _netManager?.PollEvents();

            if (DateTime.Now.Subtract(_lastRequestSent) >= RequestInterval)
            {
                SendServerListRequest();
            }
            else if (_awaitingResponse && DateTime.Now.Subtract(_lastRequestSent) >= ResponseTimeout)
            {
                _awaitingResponse = false;
                _statusLabel.text = "Failed to reach the API server.\nCheck your CSM API Server settings.";
                _statusLabel.isVisible = true;
            }
        }

        public override void OnDestroy()
        {
            _netManager?.Stop();
            base.OnDestroy();
        }

        private void SetupNetworking()
        {
            EventBasedNetListener listener = new EventBasedNetListener();
            listener.NetworkReceiveUnconnectedEvent += OnNetworkReceiveUnconnectedEvent;

            _netManager = new LiteNetLib.NetManager(listener) { UnconnectedMessagesEnabled = true };
            _netManager.Start();
        }

        private void OnNetworkReceiveUnconnectedEvent(IPEndPoint from, NetPacketReader reader, UnconnectedMessageType type)
        {
            if (type != UnconnectedMessageType.BasicMessage)
                return;

            try
            {
                // Only allow responses from the API server
                if (!Equals(from.Address, IpAddress.GetIpv4(CSM.Settings.ApiServer)))
                    return;

                CommandReceiver.Parse(reader);
            }
            catch (Exception ex)
            {
                Log.Warn($"Encountered an error while reading public server list response: {ex}");
            }
        }

        private void SendServerListRequest()
        {
            try
            {
                IPAddress apiServer = IpAddress.GetIpv4(CSM.Settings.ApiServer);
                byte[] data = ApiCommand.Serialize(new ServerListRequestCommand());
                _netManager.SendUnconnectedMessage(data, new IPEndPoint(apiServer, CSM.Settings.ApiServerPort));

                _lastRequestSent = DateTime.Now;
                _awaitingResponse = true;

                if (_rowLabels.Count == 0 && _joinButtons.Count == 0)
                {
                    _statusLabel.text = "Loading...";
                    _statusLabel.isVisible = true;
                }
            }
            catch (Exception e)
            {
                Log.Warn($"Failed to send public server list request: {e.Message}");
            }
        }

        /// <summary>
        ///     Called by ServerListResultHandler when a response arrives.
        /// </summary>
        public void OnServerListReceived(PublicServerEntry[] servers)
        {
            _awaitingResponse = false;
            UpdateRows(servers);
        }

        private void UpdateRows(PublicServerEntry[] servers)
        {
            foreach (UILabel label in _rowLabels)
            {
                Destroy(label);
            }

            foreach (UIButton button in _joinButtons)
            {
                Destroy(button);
            }

            _rowLabels.Clear();
            _joinButtons.Clear();

            if (servers.Length == 0)
            {
                _statusLabel.text = "No public servers found.";
                _statusLabel.isVisible = true;
                return;
            }

            _statusLabel.isVisible = false;

            int rowOffset = 0;
            const int rowHeight = 40;

            foreach (PublicServerEntry server in servers)
            {
                string lockIcon = server.HasPassword ? " [Protected]" : "";
                string name = string.IsNullOrEmpty(server.Name) ? "Unnamed Server" : server.Name;
                string label = $"{name}{lockIcon}\n{server.CurrentPlayers}/{server.MaxPlayers} players";

                UILabel serverLabel = _scrollablePanel.CreateLabel(label, new Vector2(5, rowOffset), 260, rowHeight);
                serverLabel.textScale = 0.8f;
                serverLabel.wordWrap = true;
                _rowLabels.Add(serverLabel);

                UIButton joinButton = _scrollablePanel.CreateButton("Join", new Vector2(280, rowOffset), 100, rowHeight);
                PublicServerEntry capturedServer = server;
                joinButton.eventClick += (component, param) => OnJoinClick(capturedServer);
                _joinButtons.Add(joinButton);

                rowOffset -= rowHeight;
            }
        }

        private void OnJoinClick(PublicServerEntry server)
        {
            if (string.IsNullOrEmpty(server.ServerToken))
            {
                Log.Warn($"Public server list entry for '{server.Name}' has no token, cannot join.");
                return;
            }

            isVisible = false;

            string username = GetSavedOrDefaultUsername();

            if (server.HasPassword)
            {
                PanelManager.ShowPanel<PasswordPromptPanel>().Show(server.ServerToken, username);
            }
            else
            {
                JoinGamePanel.JoinByToken(server.ServerToken, username);
            }
        }

        private static string GetSavedOrDefaultUsername()
        {
            ClientConfig savedConfig = null;
            ConfigData.Load(ref savedConfig, ConfigData.ClientFile);

            if (!string.IsNullOrEmpty(savedConfig?.Username))
            {
                return savedConfig.Username;
            }

            return PlatformService.active && PlatformService.personaName != null
                ? PlatformService.personaName
                : "Player";
        }
    }
}

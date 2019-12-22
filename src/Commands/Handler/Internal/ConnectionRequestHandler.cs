using System;
using System.Reflection;
using System.Threading;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using CSM.Commands.Data.Internal;
using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Status;
using CSM.Panels;
using LiteNetLib;
using NLog;

namespace CSM.Commands.Handler.Internal
{
    public class ConnectionRequestHandler : CommandHandler<ConnectionRequestCommand>
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ConnectionRequestHandler()
        {
            TransactionCmd = false;
            RelayOnServer = false;
        }

        protected override void Handle(ConnectionRequestCommand command)
        {
        }

        public void HandleOnServer(ConnectionRequestCommand command, NetPeer peer)
        {
            _logger.Info("Received connection request.");
            // Check to see if the game versions match
            if (command.GameVersion != BuildConfig.applicationVersion)
            {
                _logger.Info($"Connection rejected: Game versions {command.GameVersion} (client) and {BuildConfig.applicationVersion} (server) differ.");
                Command.SendToClient(peer, new ConnectionResultCommand
                {
                    Success = false,
                    Reason = $"Client and server have different game versions. Client: {command.GameVersion}, Server: {BuildConfig.applicationVersion}."
                });
                return;
            }

            // Check to see if the mod version matches
            Version version = Assembly.GetAssembly(typeof(Client)).GetName().Version;
            string versionString = $"{version.Major}.{version.Minor}";

            if (command.ModVersion != versionString)
            {
                _logger.Info($"Connection rejected: Mod versions {command.ModVersion} (client) and {versionString} (server) differ.");
                Command.SendToClient(peer, new ConnectionResultCommand
                {
                    Success = false,
                    Reason = $"Client and server have different CSM Mod versions. Client: {command.ModVersion}, Server: {versionString}."
                });
                return;
            }

            // Check the client username to see if anyone on the server already have a username
            bool hasExistingPlayer = MultiplayerManager.Instance.PlayerList.Contains(command.Username);
            if (hasExistingPlayer)
            {
                _logger.Info($"Connection rejected: Username {command.Username} already in use.");
                Command.SendToClient(peer, new ConnectionResultCommand
                {
                    Success = false,
                    Reason = "This username is already in use."
                });
                return;
            }

            // Check the password to see if it matches (only if the server has provided a password).
            if (!string.IsNullOrEmpty(MultiplayerManager.Instance.CurrentServer.Config.Password))
            {
                if (command.Password != MultiplayerManager.Instance.CurrentServer.Config.Password)
                {
                    _logger.Warn("Connection rejected: Invalid password provided!");
                    Command.SendToClient(peer, new ConnectionResultCommand
                    {
                        Success = false,
                        Reason = "Invalid password for this server."
                    });
                    return;
                }
            }

            SteamHelper.DLC_BitMask dlcMask = DLCHelper.GetOwnedDLCs();
            // Check both client have the same DLCs enabled
            if (!command.DLCBitMask.Equals(dlcMask))
            {
                _logger.Info($"Connection rejected: DLC bit mask {command.DLCBitMask} (client) and {dlcMask} (server) differ.");
                Command.SendToClient(peer, new ConnectionResultCommand
                {
                    Success = false,
                    Reason = "DLCs don't match",
                    DLCBitMask = dlcMask
                });
                return;
            }

            // Check that no other player is currently connecting
            bool clientJoining = false;
            foreach (Player p in MultiplayerManager.Instance.CurrentServer.ConnectedPlayers.Values)
            {
                if (p.Status != ClientStatus.Connected) {
                    clientJoining = true;
                }
            }
            if (clientJoining)
            {
                Command.SendToClient(peer, new ConnectionResultCommand
                {
                    Success = false,
                    Reason = "A client is already joining",
                    DLCBitMask = dlcMask
                });
                return;
            }

            // Add the new player as a connected player
            Player newPlayer = new Player(peer, command.Username);
            newPlayer.Status = ClientStatus.Downloading;
            MultiplayerManager.Instance.CurrentServer.ConnectedPlayers[peer.Id] = newPlayer;

            // Open status window
            ThreadHelper.dispatcher.Dispatch(() =>
            {
                ClientJoinPanel clientJoinPanel = UIView.GetAView().FindUIComponent<ClientJoinPanel>("MPClientJoinPanel");
                if (clientJoinPanel != null)
                {
                    clientJoinPanel.isVisible = true;
                    clientJoinPanel.StartCheck();
                }
                else
                {
                    clientJoinPanel = (ClientJoinPanel)UIView.GetAView().AddUIComponent(typeof(ClientJoinPanel));
                }
                clientJoinPanel.Focus();
            });

            // Inform other clients about the joining client
            Command.SendToOtherClients(new ClientJoiningCommand
            {
                JoiningFinished = false
            }, newPlayer);
            MultiplayerManager.Instance.GameBlocked = true;
            SimulationManager.instance.SimulationPaused = true;

            // Send the result command
            Command.SendToClient(peer, new ConnectionResultCommand
            {
                Success = true,
                ClientId = peer.Id
            });

            // Get a serialized version of the server world to send to the player.
            SaveHelpers.SaveServerLevel();

            new Thread(() =>
            {
                while (SaveHelpers.IsSaving())
                {
                    Thread.Sleep(100);
                }

                Command.SendToClient(peer, new WorldTransferCommand
                {
                    World = SaveHelpers.GetWorldFile()
                });

                newPlayer.Status = ClientStatus.Loading;
            }).Start();

            MultiplayerManager.Instance.CurrentServer.HandlePlayerConnect(newPlayer);
        }
    }
}

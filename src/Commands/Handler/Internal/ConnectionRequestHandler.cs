using CSM.Commands.Data.Internal;
using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Status;
using CSM.Util;
using LiteNetLib;
using System;
using System.Reflection;
using System.Threading;

namespace CSM.Commands.Handler.Internal
{
    public class ConnectionRequestHandler : CommandHandler<ConnectionRequestCommand>
    {
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
            Log.Info("Received connection request.");
            // Check to see if the game versions match
            if (command.GameVersion != command.GameVersion)
            {
                Log.Info($"Connection rejected: Game versions {command.GameVersion} (client) and {BuildConfig.applicationVersion} (server) differ.");
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

            if (command.ModVersion != command.ModVersion)
            {
                Log.Info($"Connection rejected: Mod versions {command.ModVersion} (client) and {versionString} (server) differ.");
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
                Log.Info($"Connection rejected: Username {command.Username} already in use.");
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
                    Log.Warn("Connection rejected: Invalid password provided!");
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
                Log.Info($"Connection rejected: DLC bit mask {command.DLCBitMask} (client) and {dlcMask} (server) differ.");
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
                if (p.Status != ClientStatus.Connected)
                {
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
            MultiplayerManager.Instance.CurrentServer.ConnectedPlayers[peer.Id] = newPlayer;

            // Send the result command
            Command.SendToClient(peer, new ConnectionResultCommand
            {
                Success = true,
                ClientId = peer.Id
            });

            PrepareWorldLoad(newPlayer);

            MultiplayerManager.Instance.CurrentServer.HandlePlayerConnect(newPlayer);
        }

        public static void PrepareWorldLoad(Player newPlayer)
        {
            newPlayer.Status = ClientStatus.Downloading;

            MultiplayerManager.Instance.BlockGame(newPlayer.Username);
            SimulationManager.instance.SimulationPaused = true;

            // Inform other clients about the joining client
            Command.SendToOtherClients(new ClientJoiningCommand
            {
                JoiningFinished = false,
                JoiningUsername = newPlayer.Username
            }, newPlayer);

            /*
             * Wait to get all remaining pakets processed, because unprocessed packets
             * before saving may end in an desynced game for the joining client
             */
            Thread.Sleep(2000);

            // Get a serialized version of the server world to send to the player.
            SaveHelpers.SaveServerLevel();

            new Thread(() =>
            {
                while (SaveHelpers.IsSaving())
                {
                    Thread.Sleep(100);
                }

                Command.SendToClient(newPlayer, new WorldTransferCommand
                {
                    World = SaveHelpers.GetWorldFile()
                });

                newPlayer.Status = ClientStatus.Loading;
            }).Start();
        }
    }
}

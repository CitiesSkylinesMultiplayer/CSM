﻿using CSM.Networking;
using LiteNetLib;
using System.Reflection;

namespace CSM.Commands.Handler
{
    public class ConnectionRequestHandler : CommandHandler<ConnectionRequestCommand>
    {
        public ConnectionRequestHandler()
        {
            TransactionCmd = false;
            RelayOnServer = false;
        }

        public override void Handle(ConnectionRequestCommand command) { }

        public void HandleOnServer(ConnectionRequestCommand command, NetPeer peer)
        {
            // Check to see if the game versions match
            if (command.GameVersion != BuildConfig.applicationVersion)
            {
                Command.SendToClient(peer, new ConnectionResultCommand
                {
                    Success = false,
                    Reason = $"Client and server have different game versions. Client: {command.GameVersion}, Server: {BuildConfig.applicationVersion}."
                });
                return;
            }

            // Check to see if the mod version matches
            var version = Assembly.GetAssembly(typeof(Client)).GetName().Version;
            var versionString = $"{version.Major}.{version.Minor}";

            if (command.ModVersion != versionString)
            {
                Command.SendToClient(peer, new ConnectionResultCommand
                {
                    Success = false,
                    Reason = $"Client and server have different CSM Mod versions. Client: {command.ModVersion}, Server: {versionString}."
                });
                return;
            }

            // Check the client username to see if anyone on the server already have a username
            var hasExistingPlayer = MultiplayerManager.Instance.PlayerList.Contains(command.Username);
            if (hasExistingPlayer)
            {
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
                    Command.SendToClient(peer, new ConnectionResultCommand
                    {
                        Success = false,
                        Reason = "Invalid password for this server."
                    });
                    return;
                }
            }

            var newPlayer = new Player(peer, command.Username);
            MultiplayerManager.Instance.CurrentServer.ConnectedPlayers[peer.Id] = newPlayer;

            Command.SendToClient(peer, new ConnectionResultCommand
            {
                Success = true,
                ClientId = peer.Id
            });

            MultiplayerManager.Instance.CurrentServer.HandlePlayerConnect(newPlayer);

            // Get map to send to client
        }
    }
}

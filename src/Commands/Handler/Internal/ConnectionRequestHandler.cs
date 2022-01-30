using CSM.Commands.Data.Internal;
using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Status;
using CSM.Util;
using LiteNetLib;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace CSM.Commands.Handler.Internal
{
    public class ConnectionRequestHandler : CommandHandler<ConnectionRequestCommand>
    {
        public static Player WorldLoadingPlayer = null;

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

            string joiningVersion = command.GameVersion;
            string appVersion = BuildConfig.applicationVersion;

            MatchVersionString(ref joiningVersion);
            MatchVersionString(ref appVersion);

            // Check to see if the game versions match
            if (joiningVersion != appVersion)
            {
                Log.Info($"Connection rejected: Game versions {joiningVersion} (client) and {appVersion} (server) differ.");
                Command.SendToClient(peer, new ConnectionResultCommand
                {
                    Success = false,
                    Reason = $"Client and server have different game versions. Client: {joiningVersion}, Server: {appVersion}."
                });
                return;
            }

            // Check to see if the mod version matches
            Version version = Assembly.GetAssembly(typeof(Client)).GetName().Version;
            string versionString = $"{version.Major}.{version.Minor}";

            if (command.ModVersion != versionString)
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
            //Check if join requests are enabled.
            if (MultiplayerManager.Instance.CurrentServer.Config.EnablePlayerJoinRequest)
            {

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

            // Inform other clients about the joining client
            Command.SendToOtherClients(new ClientJoiningCommand
            {
                JoiningFinished = false,
                JoiningUsername = newPlayer.Username
            }, newPlayer);

            WorldLoadingPlayer = newPlayer;

            // If the game is already paused, continue with the load process.
            // Otherwise we wait until the pause negotiation is done.
            // Note that a joining (+syncing) player is not considered during this process, but already force paused at this point.
            // See SpeedPauseHelper::StateReached()
            if (SimulationManager.instance.SimulationPaused && SpeedPauseHelper.IsStable())
            {
                AllGamesBlocked();
            }
        }

        public static void AllGamesBlocked()
        {
            Player newPlayer = WorldLoadingPlayer;
            WorldLoadingPlayer = null;

            new Thread(() =>
            {
                // Wait to get all remaining packets processed, because unprocessed packets
                // before saving may end in an desynced game for the joining client
                Thread.Sleep(2000);

                // Create game save in the main thread
                AsyncAction action = SimulationManager.instance.AddAction(SaveHelpers.SaveServerLevel);

                // Wait until the save action is queued and the game is saved
                while (!action.completed || SaveHelpers.IsSaving())
                {
                    Thread.Sleep(10);
                }

                Command.SendToClient(newPlayer, new WorldTransferCommand
                {
                    World = SaveHelpers.GetWorldFile()
                });

                newPlayer.Status = ClientStatus.Loading;
            }).Start();
        }

        /**
         * Version format:
         * x.y.z-tb(-e12)
         *
         * x.y.z is the version number
         * t is the release type (proto, alpha, beta, f)
         * b is the build number
         * and e12 is hardcoded for epic (not set on steam)
         *
         * Since epic and steam differ beginning at z, we
         * now only check for x.y to be equal.
         */
        private static void MatchVersionString(ref string version)
        {
            Match match = Regex.Match(version, @"^(\d+\.\d+)\.\d+-\w+(?:-\w+)?$");
            if (match.Success && match.Groups.Count > 1)
            {
                version = match.Groups[1].Value;
            }
        }
    }
}

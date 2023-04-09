using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Networking;
using CSM.API.Networking.Status;
using CSM.Commands.Data.Internal;
using CSM.Helpers;
using CSM.Mods;
using CSM.Networking;
using LiteNetLib;

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
                CommandInternal.Instance.SendToClient(peer, new ConnectionResultCommand
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
                CommandInternal.Instance.SendToClient(peer, new ConnectionResultCommand
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
                CommandInternal.Instance.SendToClient(peer, new ConnectionResultCommand
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
                    CommandInternal.Instance.SendToClient(peer, new ConnectionResultCommand
                    {
                        Success = false,
                        Reason = "Invalid password for this server."
                    });
                    return;
                }
            }

            SteamHelper.ExpansionBitMask expansionDlcMask = DLCHelper.GetOwnedExpansions();
            SteamHelper.ModderPackBitMask modderPackDlcMask = DLCHelper.GetOwnedModderPacks();
            // Check both client have the same DLCs enabled
            if (!command.ExpansionBitMask.Equals(expansionDlcMask))
            {
                Log.Info($"Connection rejected: DLC bit mask {command.ExpansionBitMask} + {command.ModderPackBitMask} (client) and {expansionDlcMask} + {modderPackDlcMask} (server) differ.");
                CommandInternal.Instance.SendToClient(peer, new ConnectionResultCommand
                {
                    Success = false,
                    Reason = "DLCs don't match",
                    ExpansionBitMask = expansionDlcMask,
                    ModderPackBitMask = modderPackDlcMask
                });
                return;
            }
            if (!command.ModderPackBitMask.Equals(modderPackDlcMask))
            {
                Log.Info($"Connection rejected: DLC bit mask {command.ExpansionBitMask} + {command.ModderPackBitMask} (client) and {expansionDlcMask} + {modderPackDlcMask} (server) differ.");
                CommandInternal.Instance.SendToClient(peer, new ConnectionResultCommand
                {
                    Success = false,
                    Reason = "DLCs don't match",
                    ExpansionBitMask = expansionDlcMask,
                    ModderPackBitMask = modderPackDlcMask
                });
                return;
            }

            List<string> serverMods = ModSupport.Instance.RequiredModsForSync.OrderBy(x => x).ToList();
            List<string> clientMods = (command.Mods ?? new List<string>()).OrderBy(x => x).ToList();

            if (!clientMods.SequenceEqual(serverMods) && !CSM.Settings.SkipModCompatibilityChecks.value == false)
            {
                Log.Info($"Connection rejected: List of mods [{string.Join(", ", clientMods.ToArray())}] (client) and [{string.Join(", ", serverMods.ToArray())}] (server) differ.");
                CommandInternal.Instance.SendToClient(peer, new ConnectionResultCommand
                {
                    Success = false,
                    Reason = "Mods don't match",
                    Mods = serverMods
                });
                return;
            }

            // Check that no other player is currently connecting
            bool clientJoining = MultiplayerManager.Instance.CurrentServer.ConnectedPlayers.Values.Any(p => p.Status != ClientStatus.Connected);
            if (clientJoining)
            {
                Log.Info("Connection rejected: A client is already joining.");
                CommandInternal.Instance.SendToClient(peer, new ConnectionResultCommand
                {
                    Success = false,
                    Reason = "A client is already joining"
                });
                return;
            }

            // Add the new player as a connected player
            Player newPlayer = new Player(peer, command.Username);
            MultiplayerManager.Instance.CurrentServer.ConnectedPlayers[peer.Id] = newPlayer;

            // Send the result command
            CommandInternal.Instance.SendToClient(peer, new ConnectionResultCommand
            {
                Success = true,
                ClientId = peer.Id,
                ServerToken = MultiplayerManager.Instance.CurrentServer.ServerToken
            });

            PrepareWorldLoad(newPlayer);

            MultiplayerManager.Instance.CurrentServer.HandlePlayerConnect(newPlayer);
        }

        public static void PrepareWorldLoad(Player newPlayer)
        {
            newPlayer.Status = ClientStatus.Downloading;

            MultiplayerManager.Instance.BlockGame(newPlayer.Username);

            // Inform other clients about the joining client
            CommandInternal.Instance.SendToOtherClients(new ClientJoiningCommand
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

                CommandInternal.Instance.SendToClient(newPlayer, new WorldTransferCommand
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

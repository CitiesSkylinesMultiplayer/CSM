using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using ColossalFramework;
using ColossalFramework.PlatformServices;
using ColossalFramework.Threading;
using CSM.API;
using CSM.Helpers.Steamworks;
using CSM.Networking;
using CSM.Networking.Config;
using CSM.Panels;

namespace CSM.Helpers
{
    public class SteamHelpers
    {
        public static SteamHelpers Instance { get; private set; }

        private IntPtr _friendsPtr;
        private Callback<GameRichPresenceJoinRequested_t> _gameRichPresenceJoinRequested;
        private readonly Dictionary<string, string> _richPresenceValues = new Dictionary<string, string>();
        private Thread _refreshThread;
        private static bool _use64Bit = true;

        public static bool Init()
        {
            Instance = new SteamHelpers();
            try
            {
                bool res = SteamAPI_Init();
                if (!res) return false;

                // Setup context
                int hSteamUser = SteamAPI_GetHSteamUser();
                if (hSteamUser == 0) return false;

                int hSteamPipe = SteamAPI_GetHSteamPipe();
                if (hSteamPipe == 0) return false;

                IntPtr client = SteamClient();
                if (client == IntPtr.Zero) return false;

                Instance._friendsPtr = SteamAPI_ISteamClient_GetISteamFriends(client, hSteamUser, hSteamPipe, "SteamFriends015");
                if (Instance._friendsPtr == IntPtr.Zero) return false;

                Instance._gameRichPresenceJoinRequested = Callback<GameRichPresenceJoinRequested_t>.Create(OnGameRichPresenceJoinRequested);

                Instance._refreshThread = new Thread(() =>
                {
                    while (true)
                    {
                        Instance.RefreshRichPresence();
                        Thread.Sleep(TimeSpan.FromMinutes(2));
                    }
                });
                Instance._refreshThread.Start();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void OnGameRichPresenceJoinRequested(GameRichPresenceJoinRequested_t pCallback)
        {
            string[] token = pCallback.m_rgchConnect.Split('=');
            if (token[0] != "csm") return;
            Instance.JoinFromSteam(token[1], true);
        }

        public void CheckCommandLineCallback()
        {
            Singleton<LoadingManager>.instance.m_introLoaded -= CheckCommandLineCallback;
            CheckCommandLine(false);
        }

        public void CheckCommandLine(bool checkLoadingManager)
        {
            // Join game if cmd line argument from steam overlay is set
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (!arg.StartsWith("csm=")) continue;

                string[] argSplit = arg.Split('=');
                if (argSplit.Length == 2 && CSM.IsSteamPresent)
                {
                    JoinFromSteam(argSplit[1], checkLoadingManager);
                }
            }
        }

        private void JoinFromSteam(string token, bool checkLoadingManager)
        {
            if (checkLoadingManager && Singleton<LoadingManager>.exists && (Singleton<LoadingManager>.instance.m_currentlyLoading ||
                                                                        Singleton<LoadingManager>.instance.m_loadingComplete))
            {
                // Not allowed while loading or in game
                MessagePanel panel = PanelManager.ShowPanel<MessagePanel>();
                panel.DisplayJoiningNotAllowed();
                return;
            }

            Log.Info("Join request for " + token);

            JoinGamePanel join = PanelManager.ShowPanel<JoinGamePanel>();
            join.SetConnecting();

            MultiplayerManager.Instance.CurrentClient.StartMainMenuEventProcessor();
            MultiplayerManager.Instance.ConnectToServer(new ClientConfig(token, PlatformService.personaName),
                (success) =>
                {
                    if (success)
                    {
                        // See WorldTransferHandler for actual loading
                        ThreadHelper.dispatcher.Dispatch(() =>
                        {
                            MultiplayerManager.Instance.BlockGameFirstJoin();
                        });
                    }
                    else
                    {
                        ThreadHelper.dispatcher.Dispatch(() =>
                        {
                            JoinGamePanel panel = PanelManager.ShowPanel<JoinGamePanel>();
                            panel.FillFieldsOnError(token, PlatformService.personaName,
                                MultiplayerManager.Instance.CurrentClient.ConnectionMessage);
                        });
                    }
                });
        }

        public void Shutdown()
        {
            _gameRichPresenceJoinRequested.Unregister();
            _refreshThread?.Abort();
            // We can't shutdown the Steam API here as Cities might continue using it (also it crashes the game)
        }

        /// <summary>
        ///     Open the Steam Overlay to the friends menu.
        /// </summary>
        public void OpenFriendOverlay()
        {
            if (_friendsPtr == IntPtr.Zero) return;

            SteamAPI_ISteamFriends_ActivateGameOverlay(_friendsPtr, "friends");
        }

        /// <summary>
        ///     Set the group size of the steam rich presence.
        /// </summary>
        /// <param name="size">The amount of players in the current game session.</param>
        public void SetGroupSize(int size)
        {
            SetRichPresence("steam_player_group_size", size.ToString());
        }

        /// <summary>
        ///     Set steam rich presence values when playing on a server.
        /// </summary>
        /// <param name="token">The current server's token.</param>
        public void SetPlayingOnServer(string token)
        {
            SetRichPresence("status", "Playing on a CSM server");
            SetRichPresence("connect", "csm=" + token);
            SetRichPresence("steam_player_group", token);
        }

        /// <summary>
        ///     Set a Steam rich presence value.
        /// </summary>
        /// <param name="key">The key, e.g "steam_player_group"</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="Exception">If the invocation failed.</exception>
        private void SetRichPresence(string key, string value)
        {
            _richPresenceValues[key] = value;
            Log.Debug($"Steam RichPresence: {key}={value}");
            if (_friendsPtr == IntPtr.Zero) return;

            bool res = SteamAPI_ISteamFriends_SetRichPresence(_friendsPtr, key, value);
            if (!res)
            {
                throw new Exception("SteamAPI_ISteamFriends_SetRichPresence failed");
            }
        }

        /// <summary>
        ///     Clear all Steam rich presence values.
        /// </summary>
        public void ClearRichPresence()
        {
            _richPresenceValues.Clear();
            SteamAPI_ISteamFriends_ClearRichPresence(_friendsPtr);
        }

        /// <summary>
        ///     Refresh the Steam Rich Presence values.
        ///     This is sometimes needed when the Steam
        ///     chat gets interrupted.
        /// </summary>
        private void RefreshRichPresence()
        {
            Log.Debug("Refreshing rich presence");
            if (_friendsPtr == IntPtr.Zero) return;

            foreach (KeyValuePair<string, string> presence in _richPresenceValues)
            {
                SteamAPI_ISteamFriends_SetRichPresence(_friendsPtr, presence.Key, presence.Value);
            }
        }

        // NATIVE METHODS
        // We need to decide between 64 and 32bit as it seems to be different on Windows/Linux platforms
        private static bool SteamAPI_Init()
        {
            try
            {
                return SteamAPI_Init_64();
            }
            catch (DllNotFoundException)
            {
                _use64Bit = false;
                return SteamAPI_Init_32();
            }
        }
        private static bool SteamAPI_ISteamFriends_SetRichPresence(IntPtr instancePtr, string pchKey, string pchValue)
        {
            return _use64Bit ? SteamAPI_ISteamFriends_SetRichPresence_64(instancePtr, pchKey, pchValue) : SteamAPI_ISteamFriends_SetRichPresence_32(instancePtr, pchKey, pchValue);
        }
        private static void SteamAPI_ISteamFriends_ClearRichPresence(IntPtr instancePtr)
        {
            if (_use64Bit) SteamAPI_ISteamFriends_ClearRichPresence_64(instancePtr);
            else SteamAPI_ISteamFriends_ClearRichPresence_32(instancePtr);
        }
        private static IntPtr SteamAPI_ISteamClient_GetISteamFriends(IntPtr instancePtr, int hStreamUser, int hSteamPipe, string pchVersion)
        {
            return _use64Bit ? SteamAPI_ISteamClient_GetISteamFriends_64(instancePtr, hStreamUser, hSteamPipe, pchVersion) : SteamAPI_ISteamClient_GetISteamFriends_32(instancePtr, hStreamUser, hSteamPipe, pchVersion);
        }
        private static int SteamAPI_GetHSteamUser()
        {
            return _use64Bit ? SteamAPI_GetHSteamUser_64() : SteamAPI_GetHSteamUser_32();
        }
        private static int SteamAPI_GetHSteamPipe()
        {
            return _use64Bit ? SteamAPI_GetHSteamPipe_64() : SteamAPI_GetHSteamPipe_32();
        }
        private static IntPtr SteamClient()
        {
            return _use64Bit ? SteamClient_64() : SteamClient_32();
        }
        public static void SteamAPI_RegisterCallback(IntPtr pCallback, int iCallback)
        {
            if (_use64Bit) SteamAPI_RegisterCallback_64(pCallback, iCallback);
            else SteamAPI_RegisterCallback_32(pCallback, iCallback);
        }
        public static void SteamAPI_UnregisterCallback(IntPtr pCallback)
        {
            if (_use64Bit) SteamAPI_UnregisterCallback_64(pCallback);
            else SteamAPI_UnregisterCallback_32(pCallback);
        }
        public static void SteamAPI_RegisterCallResult(IntPtr pCallback, ulong hAPICall)
        {
            if (_use64Bit) SteamAPI_RegisterCallResult_64(pCallback, hAPICall);
            else SteamAPI_RegisterCallResult_32(pCallback, hAPICall);
        }
        public static void SteamAPI_UnregisterCallResult(IntPtr pCallback, ulong hAPICall)
        {
            if (_use64Bit) SteamAPI_UnregisterCallResult_64(pCallback, hAPICall);
            else SteamAPI_UnregisterCallResult_32(pCallback, hAPICall);
        }
        private static void SteamAPI_ISteamFriends_ActivateGameOverlay(IntPtr instancePtr, string pchDialog)
        {
            if (_use64Bit) SteamAPI_ISteamFriends_ActivateGameOverlay_64(instancePtr, pchDialog);
            else SteamAPI_ISteamFriends_ActivateGameOverlay_32(instancePtr, pchDialog);
        }

        [DllImport("steam_api", EntryPoint = "SteamAPI_Init", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SteamAPI_Init_32();

        [DllImport("steam_api", EntryPoint = "SteamAPI_ISteamFriends_SetRichPresence", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SteamAPI_ISteamFriends_SetRichPresence_32(IntPtr instancePtr, [MarshalAs(UnmanagedType.LPStr)] string pchKey, [MarshalAs(UnmanagedType.LPStr)] string pchValue);

        [DllImport("steam_api", EntryPoint = "SteamAPI_ISteamFriends_ClearRichPresence", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SteamAPI_ISteamFriends_ClearRichPresence_32(IntPtr instancePtr);

        [DllImport("steam_api", EntryPoint = "SteamAPI_ISteamClient_GetISteamFriends", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SteamAPI_ISteamClient_GetISteamFriends_32(IntPtr instancePtr, int hStreamUser, int hSteamPipe, [MarshalAs(UnmanagedType.LPStr)] string pchVersion);

        [DllImport("steam_api", EntryPoint = "SteamAPI_GetHSteamUser", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SteamAPI_GetHSteamUser_32();

        [DllImport("steam_api", EntryPoint = "SteamAPI_GetHSteamPipe", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SteamAPI_GetHSteamPipe_32();

        [DllImport("steam_api", EntryPoint = "SteamClient", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SteamClient_32();

        [DllImport("steam_api", EntryPoint = "SteamAPI_RegisterCallback", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SteamAPI_RegisterCallback_32(IntPtr pCallback, int iCallback);

        [DllImport("steam_api", EntryPoint = "SteamAPI_UnregisterCallback", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SteamAPI_UnregisterCallback_32(IntPtr pCallback);

        [DllImport("steam_api", EntryPoint = "SteamAPI_RegisterCallResult", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SteamAPI_RegisterCallResult_32(IntPtr pCallback, ulong hAPICall);

        [DllImport("steam_api", EntryPoint = "SteamAPI_UnregisterCallResult", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SteamAPI_UnregisterCallResult_32(IntPtr pCallback, ulong hAPICall);

        [DllImport("steam_api", EntryPoint = "SteamAPI_ISteamFriends_ActivateGameOverlay", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SteamAPI_ISteamFriends_ActivateGameOverlay_32(IntPtr instancePtr, [MarshalAs(UnmanagedType.LPStr)] string pchDialog);

        [DllImport("steam_api64", EntryPoint = "SteamAPI_Init", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SteamAPI_Init_64();

        [DllImport("steam_api64", EntryPoint = "SteamAPI_ISteamFriends_SetRichPresence", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SteamAPI_ISteamFriends_SetRichPresence_64(IntPtr instancePtr, [MarshalAs(UnmanagedType.LPStr)] string pchKey, [MarshalAs(UnmanagedType.LPStr)] string pchValue);

        [DllImport("steam_api64", EntryPoint = "SteamAPI_ISteamFriends_ClearRichPresence", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SteamAPI_ISteamFriends_ClearRichPresence_64(IntPtr instancePtr);

        [DllImport("steam_api64", EntryPoint = "SteamAPI_ISteamClient_GetISteamFriends", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SteamAPI_ISteamClient_GetISteamFriends_64(IntPtr instancePtr, int hStreamUser, int hSteamPipe, [MarshalAs(UnmanagedType.LPStr)] string pchVersion);

        [DllImport("steam_api64", EntryPoint = "SteamAPI_GetHSteamUser", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SteamAPI_GetHSteamUser_64();

        [DllImport("steam_api64", EntryPoint = "SteamAPI_GetHSteamPipe", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SteamAPI_GetHSteamPipe_64();

        [DllImport("steam_api64", EntryPoint = "SteamClient", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SteamClient_64();

        [DllImport("steam_api64", EntryPoint = "SteamAPI_RegisterCallback", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SteamAPI_RegisterCallback_64(IntPtr pCallback, int iCallback);

        [DllImport("steam_api64", EntryPoint = "SteamAPI_UnregisterCallback", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SteamAPI_UnregisterCallback_64(IntPtr pCallback);

        [DllImport("steam_api64", EntryPoint = "SteamAPI_RegisterCallResult", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SteamAPI_RegisterCallResult_64(IntPtr pCallback, ulong hAPICall);

        [DllImport("steam_api64", EntryPoint = "SteamAPI_UnregisterCallResult", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SteamAPI_UnregisterCallResult_64(IntPtr pCallback, ulong hAPICall);

        [DllImport("steam_api64", EntryPoint = "SteamAPI_ISteamFriends_ActivateGameOverlay", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SteamAPI_ISteamFriends_ActivateGameOverlay_64(IntPtr instancePtr, [MarshalAs(UnmanagedType.LPStr)] string pchDialog);
    }
}

using System;
using System.Runtime.InteropServices;

namespace CSM.Helpers
{
    public static class SteamHelpers
    {
        private static IntPtr _friendsPtr;

        public static bool Init()
        {
            var res = SteamAPI_Init();
            if (!res) return false;

            // Setup context
            var hSteamUser = SteamAPI_GetHSteamUser();
            if (hSteamUser == 0) throw new Exception("SteamAPI_GetHSteamUser returned 0");

            var hSteamPipe = SteamAPI_GetHSteamPipe();
            if (hSteamPipe == 0) throw new Exception("SteamAPI_GetHSteamUser returned 0");

            var client = SteamClient();
            if (client == IntPtr.Zero) throw new Exception("SteamAPI_GetHSteamUser returned 0");

            _friendsPtr = SteamAPI_ISteamClient_GetISteamFriends(client, hSteamUser, hSteamPipe, "SteamFriends015");
            if (_friendsPtr == IntPtr.Zero) throw new Exception("SteamAPI_ISteamClient_GetISteamFriends returned 0");

            return true;
        }

        public static void SetRichPresence(string key, string value)
        {
            if (_friendsPtr == IntPtr.Zero) return;

            SteamAPI_ISteamFriends_SetRichPresence(_friendsPtr, key, value);
        }

        // NATIVE METHODS

        [DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SteamAPI_Init();

        [DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SteamFriends();

        [DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SteamAPI_ISteamFriends_SetRichPresence(IntPtr instancePtr, [MarshalAs(UnmanagedType.LPStr)] string pchKey, [MarshalAs(UnmanagedType.LPStr)] string pchValue);

        [DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SteamAPI_ISteamClient_GetISteamFriends(IntPtr instancePtr, int hStreamUser, int hSteamPipe, [MarshalAs(UnmanagedType.LPStr)] string pchVersion);

        [DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SteamAPI_GetHSteamUser();

        [DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SteamAPI_GetHSteamPipe();

        [DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SteamClient();

    }
}

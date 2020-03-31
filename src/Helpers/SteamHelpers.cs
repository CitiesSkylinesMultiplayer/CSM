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


            // Setup steam friends
            _friendsPtr = SteamFriends();
            if (_friendsPtr == IntPtr.Zero) 
                throw new Exception("SteamFriends returned 0");

            return true;

        }

        public static void SetRichPresence(string key, string value)
        {
            if (_friendsPtr == IntPtr.Zero) return;
        }

        // EXTERN

        [DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern bool SteamAPI_Init();

        [DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr SteamFriends();

        [DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern bool SteamAPI_ISteamFriends_SetRichPresence(IntPtr instancePtr, [MarshalAs(UnmanagedType.LPStr)] string pchKey, [MarshalAs(UnmanagedType.LPStr)] string pchValue);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CSM.Helpers
{
    public static class SteamHelpers
    {
        public static bool Init()
        {
            return SteamAPI_Init();
        }

        public static void SetRichPresence(string key, string value)
        {

        }


        // EXTERN

        [DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern bool SteamAPI_Init();
    }
}

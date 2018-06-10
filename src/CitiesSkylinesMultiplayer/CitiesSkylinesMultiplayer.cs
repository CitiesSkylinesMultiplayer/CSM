using System;
using ColossalFramework.Plugins;
using UnityEngine;

namespace CitiesSkylinesMultiplayer
{
    public class CitiesSkylinesMultiplayer : ICities.IUserMod
    {
        public static bool IsUnity = true;

        public string Name => "Multiplayer Mod";

        public string Description => "Muiltiplayer mod for Cities: Skylines.";

        public static void Log(string message)
        {
            if (IsUnity)
            {
                Debug.Log($"[CSM] {message}");
            }
            else
            {
                Console.WriteLine($"[CSM] {message}");
            }
        }
    }
}
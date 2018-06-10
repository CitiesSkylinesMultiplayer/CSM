using System;
using ColossalFramework.Plugins;
using UnityEngine;

namespace CitiesSkylinesMultiplayer
{
    public class CitiesSkylinesMultiplayer : ICities.IUserMod
    {
        public string Name => "Multiplayer Mod";

        public string Description => "Muiltiplayer mod for Cities: Skylines.";

        public static void Log(string message)
        {
            Debug.Log($"[CSM] {message}");
            Console.WriteLine($"[CSM] {message}");
        }
    }
}
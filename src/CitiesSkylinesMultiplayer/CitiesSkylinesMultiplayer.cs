using System;
using ColossalFramework.Plugins;

namespace CitiesSkylinesMultiplayer
{
    public class CitiesSkylinesMultiplayer : ICities.IUserMod
    {
        public string Name => "Cities: Skylines - Multiplayer Mod";

        public string Description => "Muiltiplayer mod for Cities: Skylines.";

        public static void Log(PluginManager.MessageType type, string message)
        {
            // Run in game console
            DebugOutputPanel.AddMessage(type, $"[CSM] {message}");

            // Run in cmd console
            Console.WriteLine($"[CSM] {message}");
        }
    }
}
using ColossalFramework.Plugins;

namespace CitiesSkylinesMultiplayer
{
    public class CitiesSkylinesMultiplayer : ICities.IUserMod
    {
        public string Name => "Cities: Skylines - Multiplayer Mod";

        public string Description => "Muiltiplayer mod for Cities: Skylines.";

        public static void Log(PluginManager.MessageType type, string message)
        {
            DebugOutputPanel.AddMessage(type, $"[CSM] {message}");
        }
    }
}
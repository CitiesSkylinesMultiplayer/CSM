using ColossalFramework.Plugins;

namespace Tango
{
    public class TangoMod : ICities.IUserMod
    {
        public string Name => "Tango: Multiplayer Mod";

        public string Description => "Muiltiplayer mod for Cities: Skylines.";

        public static void Log(PluginManager.MessageType type, string message)
        {
            DebugOutputPanel.AddMessage(type, $"[Tango] {message}");
        }
    }
}

using CSM.API;

namespace SampleExternalMod
{
    class SampleExternalModSupport : Connection
    {
        public SampleExternalModSupport()
        {
            Name = "Sample External Mod";
            Enabled = false;

            // Check if mod is loaded. Example:
            //bool modLoaded = Singleton<PluginManager>.instance.GetPluginsInfo().Any(info => info.isEnabled && info.name.Equals("SampleExternalMod"));
            //if (!modLoaded) return;

            ModClass = typeof(SampleUserMod);
            Enabled = true;

            // Add Assembly to scan for command handlers
            CommandAssemblies.Add(typeof(SampleExternalModSupport).Assembly);
        }

        public override void RegisterHandlers()
        {
            // Register any handlers/patches
            Patcher.PatchAll();
        }

        public override void UnregisterHandlers()
        {
            // Unregister all registered handlers/patches
            Patcher.UnpatchAll();
        }
    }
}

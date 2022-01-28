using System;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Plugins;
using CSM.API;

namespace CSM.TMPE
{
    class TMPESupport : Connection
    {
        public TMPESupport()
        {
            Name = "TMPE";
            Enabled = false;

            Type type = Type.GetType("TrafficManager.Lifecycle.TrafficManagerMod", false);
            if (type == null) return;

            bool tmpeEnabled = Singleton<PluginManager>.instance.GetPluginsInfo().Any(info => info.isEnabled && info.name.Equals("TMPE"));

            if (!tmpeEnabled) return;

            Enabled = true;
            CommandAssemblies.Add(Assembly.GetExecutingAssembly());

            TMPEListener.Listen();
        }

        ~TMPESupport()
        {
            if (Enabled)
            {
                TMPEListener.Unregister();
            }
        }
    }
}

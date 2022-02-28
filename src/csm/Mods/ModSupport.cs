using CSM.API;
using CSM.Helpers;
using CSM.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Plugins;
using CSM.Commands;
using ICities;

namespace CSM.Mods
{
    internal class ModSupport
    {
        private static ModSupport _instance;
        public static ModSupport Instance => _instance ?? (_instance = new ModSupport());

        public List<Connection> ConnectedMods { get; } = new List<Connection>();

        public List<string> ConnectModNames
        {
            get
            {
                return ConnectedMods.Select(connection => connection.Name).ToList();
            }
        }

        public void Init()
        {
            Singleton<PluginManager>.instance.eventPluginsChanged += LoadModConnections;
            Singleton<PluginManager>.instance.eventPluginsStateChanged += LoadModConnections;
        }

        private void LoadModConnections()
        {
            ConnectedMods.Clear();
            IEnumerable<Type> handlers = AssemblyHelper.FindClassesInMods(typeof(Connection));

            foreach (Type handler in handlers)
            {
                if (handler.IsAbstract)
                {
                    continue;
                }

                Connection connectionInstance = (Connection)Activator.CreateInstance(handler);

                if (connectionInstance != null)
                {
                    if (connectionInstance.Enabled)
                    {
                        Log.Info($"Mod connected: {connectionInstance.Name}");
                        ConnectedMods.Add(connectionInstance);
                    }
                    else
                    {
                        Log.Debug($"Mod support for {connectionInstance.Name} found but not enabled.");
                    }
                }
                else
                {
                    Log.Warn("Mod failed to instantiate.");
                }
            }

            // Refresh data model
            CommandInternal.Instance.RefreshModel();
        }

        public void OnLevelLoaded(LoadMode mode)
        {
            foreach (Connection mod in ConnectedMods)
            {
                mod.RegisterHandlers(mode);
            }
        }

        public void OnLevelUnloading()
        {
            foreach (Connection mod in ConnectedMods)
            {
                mod.UnregisterHandlers();
            }
        }

        public void DestroyConnections()
        {
            ConnectedMods.Clear();
            ConnectedMods.TrimExcess();
            
            Singleton<PluginManager>.instance.eventPluginsChanged -= LoadModConnections;
            Singleton<PluginManager>.instance.eventPluginsStateChanged -= LoadModConnections;
        }
    }
}

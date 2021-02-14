using System.Linq;
using ColossalFramework;
using ColossalFramework.Plugins;

namespace CSM.Util
{
    public static class ModCompat
    {
        private static readonly string[] DisableChirperNames = { "MyFirstMod.DestroyChirperMod", "RemoveChirper.RemoveChirper" };
        public static bool HasDisableChirperMod {
            get
            {
                if (!_hasDisableChirperMod.HasValue)
                {
                    foreach (PluginManager.PluginInfo info in Singleton<PluginManager>.instance.GetPluginsInfo())
                    {
                        if (info.isEnabled && DisableChirperNames.Contains(info.name))
                        {
                            _hasDisableChirperMod = true;
                            break;
                        }
                    }

                    if (!_hasDisableChirperMod.HasValue)
                    {
                        _hasDisableChirperMod = false;
                    }
                }

                return _hasDisableChirperMod.Value;
            }
        }

        private static bool? _hasDisableChirperMod;


        /// <summary>
        ///     Register event handlers to clear caches when mods or mod states were changed
        /// </summary>
        public static void Init()
        {
            Singleton<PluginManager>.instance.eventPluginsChanged += () =>
            {
                _hasDisableChirperMod = null;
            };
            Singleton<PluginManager>.instance.eventPluginsStateChanged += () =>
            {
                _hasDisableChirperMod = null;
            };
        }
    }
}

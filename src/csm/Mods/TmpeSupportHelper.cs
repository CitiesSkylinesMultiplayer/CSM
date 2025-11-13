using System;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Plugins;
using ICities;

namespace CSM.Mods
{
    internal static class TmpeSupportHelper
    {
        public const string TmpeSyncWorkshopLink = "https://steamcommunity.com/sharedfiles/filedetails/?id=3600743038";

        private static readonly string[] TmpeTypeNames =
        {
            "TrafficManager.Lifecycle.TrafficManagerMod"
        };

        private static readonly string[] TmpeSyncTypeNames =
        {
            "CSM.TmpeSync.Mod",
            "CSM.TmpeSync.TmpeSyncMod"
        };

        private static bool? _hasTmpeSync;

        public static bool IsTmpeMod(string typeName)
        {
            return !string.IsNullOrEmpty(typeName) && TmpeTypeNames.Contains(typeName);
        }

        public static bool HasTmpeSyncMod()
        {
            if (_hasTmpeSync.HasValue)
            {
                return _hasTmpeSync.Value;
            }

            foreach (PluginManager.PluginInfo info in Singleton<PluginManager>.instance.GetPluginsInfo())
            {
                if (!info.isEnabled)
                {
                    continue;
                }

                IUserMod modInstance = info.userModInstance as IUserMod;
                string typeName = modInstance?.GetType().ToString();
                if (IsTmpeSyncMod(typeName))
                {
                    _hasTmpeSync = true;
                    return true;
                }
            }

            _hasTmpeSync = false;
            return false;
        }

        public static void InvalidateCache()
        {
            _hasTmpeSync = null;
        }

        private static bool IsTmpeSyncMod(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return false;
            }

            if (TmpeSyncTypeNames.Contains(typeName))
            {
                return true;
            }

            return typeName.StartsWith("CSM.TmpeSync", StringComparison.Ordinal);
        }
    }
}

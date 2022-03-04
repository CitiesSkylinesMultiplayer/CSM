using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using CSM.API;
using CSM.Helpers;
using ICities;
using UnityEngine;

namespace CSM.Mods
{
    public enum ModSupportType
    {
        Unknown,
        Unsupported,
        Supported,
        KnownWorking,
    }

    public struct ModSupportStatus
    {
        public ModSupportStatus(string name, string typeName, ModSupportType type, bool clientSide)
        {
            Name = name;
            TypeName = typeName;
            Type = type;
            ClientSide = clientSide;
        }

        public string Name { get; }
        public string TypeName { get; }
        public ModSupportType Type { get; }
        public bool ClientSide { get; }
    }

    public static class ModCompat
    {
        private static readonly string[] _clientSideMods = { "LoadingScreenMod.Mod", "MyFirstMod.DestroyChirperMod", "RemoveChirper.RemoveChirper", "ChirpRemover.ChirpRemover", "MoreAspectRatios.MoreAspectRatios" };
        private static readonly string[] _ignoredMods = { "CitiesHarmony.Mod" };

        private static readonly string[] _knownToWork = { "LoadingScreenMod.Mod", "MoreAspectRatios.MoreAspectRatios" };
        private static readonly string[] _unsupportedMods = { "TrafficManager.Lifecycle.TrafficManagerMod" };

        private static readonly string[] _disableChirperNames = { "MyFirstMod.DestroyChirperMod", "RemoveChirper.RemoveChirper", "ChirpRemover.ChirpRemover" };
        public static bool HasDisableChirperMod {
            get
            {
                if (_hasDisableChirperMod.HasValue) return _hasDisableChirperMod.Value;

                foreach (PluginManager.PluginInfo info in Singleton<PluginManager>.instance.GetPluginsInfo())
                {
                    if (info.isEnabled && _disableChirperNames.Contains(info.userModInstance.GetType().ToString()))
                    {
                        _hasDisableChirperMod = true;
                        return true;
                    }
                }

                _hasDisableChirperMod = false;
                return false;
            }
        }

        private static bool? _hasDisableChirperMod;

        private static IEnumerable<ModSupportStatus> GetModSupport()
        {
            foreach (PluginManager.PluginInfo info in Singleton<PluginManager>.instance.GetPluginsInfo())
            {
                // Skip disabled mods
                if (!info.isEnabled)
                    continue;

                IUserMod modInstance = info.userModInstance as IUserMod;

                // Skip CSM itself
                if (modInstance?.GetType() == typeof(CSM))
                    continue;

                // Skip built-in mods (TODO: Check if actually everything works with them)
                if (info.isBuiltin)
                    continue;

                string modInstanceName = modInstance?.GetType().ToString();

                // Skip ignored mods
                if (_ignoredMods.Contains(modInstanceName))
                    continue;

                bool isClientSide = _clientSideMods.Contains(modInstanceName);

                // Explicitly supported mods
                if (_disableChirperNames.Contains(modInstanceName))
                {
                    yield return new ModSupportStatus(modInstance?.Name, modInstanceName, ModSupportType.Supported, isClientSide);
                    continue;
                }

                // Mods known to work
                if (_knownToWork.Contains(modInstanceName))
                {
                    yield return new ModSupportStatus(modInstance?.Name, modInstanceName, ModSupportType.KnownWorking, isClientSide);
                    continue;
                }

                // Mods with loaded multiplayer support
                if (ModSupport.Instance.ConnectedMods.Select(mod => mod.ModClass)
                    .Contains(modInstance?.GetType()))
                {
                    yield return new ModSupportStatus(modInstance?.Name, modInstanceName, ModSupportType.Supported, isClientSide);
                    continue;
                }

                // Decide between unsupported and unknown
                if (_unsupportedMods.Contains(modInstanceName))
                {
                    yield return new ModSupportStatus(modInstance?.Name, modInstanceName, ModSupportType.Unsupported, isClientSide);
                    continue;
                }

                yield return new ModSupportStatus(modInstance?.Name, modInstanceName, ModSupportType.Unknown, isClientSide);
            }
        }

        public static void BuildModInfo(UIPanel panel)
        {
            UIComponent modInfoPanel = panel.Find("modInfoPanel");
            if (modInfoPanel != null)
            {
                modInfoPanel.Remove();
            }

            IEnumerable<ModSupportStatus> modSupport = GetModSupport().ToList();
            if (!modSupport.Any())
            {
                panel.width = 360;
                return;
            }

            modInfoPanel = panel.AddUIComponent<UIScrollablePanel>();
            modInfoPanel.name = "modInfoPanel";
            modInfoPanel.width = 340;
            modInfoPanel.height = 500;
            modInfoPanel.position = new Vector2(370, -60);

            panel.width = 720;
            modInfoPanel.CreateLabel("Mod Support", new Vector2(0, 0), 340, 20);

            Log.Debug($"Mod support: {string.Join(", ", modSupport.Select(m => $"{m.TypeName} ({m.Type})").ToArray())}");
            int y = -50;
            foreach (ModSupportStatus mod in modSupport)
            {
                string modName = mod.Name.Length > 30 ? mod.Name.Substring(0, 30) + "..." : mod.Name;
                UILabel nameLabel = modInfoPanel.CreateLabel($"{modName}", new Vector2(10, y), 340, 20);
                nameLabel.textScale = 0.9f;

                string message;
                Color32 labelColor;
                switch (mod.Type)
                {
                    case ModSupportType.Supported:
                        message = "Supported";
                        labelColor = new Color32(0, 255, 0, 255);
                        break;
                    case ModSupportType.Unsupported:
                        message = "Unsupported";
                        labelColor = new Color32(170, 0, 0, 255);
                        break;
                    case ModSupportType.KnownWorking:
                        message = "Known to work";
                        labelColor = new Color32(160, 255, 0, 255);
                        break;
                    case ModSupportType.Unknown:
                        message = "Unknown";
                        labelColor = new Color32(255, 100, 0, 255);
                        break;
                    default:
                        continue;
                }

                if (mod.ClientSide)
                {
                    message += " (Client side mod)";
                }

                UILabel label = modInfoPanel.CreateLabel(message, new Vector2(10, y - 20), 340, 20);
                label.textColor = labelColor;
                label.textScale = 0.9f;
                y -= 50;
            }
        }

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

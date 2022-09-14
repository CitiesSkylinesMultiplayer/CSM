using System.Threading;
using ColossalFramework.UI;
using CSM.API;
using CSM.Injections;
using CSM.Mods;
using ICities;

namespace CSM.Panels
{
    public static class SettingsPanel
    {
        public static void Build(UIHelperBase helper, Settings settings)
        {
            var chatGroup = helper.AddGroup("Chat");

            UICheckBox useChirper = (UICheckBox) chatGroup.AddCheckbox("Use Chirper as chat", settings.UseChirper,
                c => { settings.UseChirper = c; });
            UICheckBox regularChirper = (UICheckBox) chatGroup.AddCheckbox("Print regular Chirper messages", settings.PrintChirperMsgs.value,
                c => { settings.PrintChirperMsgs.value = c; });

            if (ModCompat.HasDisableChirperMod)
            {
                useChirper.readOnly = true;
                useChirper.tooltip = "Disable Chirper mod detected. Chirper chat is not available.";
                regularChirper.readOnly = true;
                regularChirper.tooltip = "Disable Chirper mod detected. Chirper chat is not available.";
            }

            var advancedGroup = helper.AddGroup("Advanced");

            UICheckBox cb = (UICheckBox)advancedGroup.AddCheckbox("Enable debug logging", settings.DebugLogging.value,
                c => {
                    settings.DebugLogging.value = c;
                    Log.Instance.LogDebug = c;
                });
            cb.tooltip = "Note: This may cause excessive logging and slow down the game!";

            UICheckBox cb2 = (UICheckBox)advancedGroup.AddCheckbox("Skip mod compatibility checks", settings.SkipModCompatibilityChecks.value,
                c => {
                    settings.SkipModCompatibilityChecks.value = c;
                });
            cb2.tooltip = "Use this at your own risk! Only applies when hosting";

            advancedGroup.AddButton("Show Release Notes", () =>
            {
                MessagePanel panel = PanelManager.ShowPanel<MessagePanel>();
                if (panel)
                    panel.DisplayReleaseNotes();
            });

            advancedGroup.AddButton("Check for updates", () =>
            {
                new Thread(() => MainMenuHandler.CheckForUpdate(true)).Start();
            });
        }
    }
}

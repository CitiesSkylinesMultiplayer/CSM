using System;
using System.Threading;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using CSM.API;
using CSM.Injections;
using CSM.Mods;
using CSM.Util;
using ICities;

namespace CSM.Panels
{
    public static class SettingsPanel
    {
        public static void Build(UIHelperBase helper, Settings settings)
        {
            UIHelperBase chatGroup = helper.AddGroup("Chat");

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

            UIHelperBase advancedGroup = helper.AddGroup("Advanced");

            UICheckBox cb = (UICheckBox)advancedGroup.AddCheckbox("Enable debug logging", settings.DebugLogging.value,
                c => {
                    settings.DebugLogging.value = c;
                    Log.Instance.LogDebug = c;
                });
            cb.tooltip = "Note: This may cause excessive logging and slow down the game!";

            UITextField urlInput = null;
            urlInput = (UITextField) advancedGroup.AddTextfield("CSM API Server", settings.ApiServer.value, text => {}, url =>
            {
                new Thread(() =>
                {
                    try
                    {
                        new CSMWebClient().DownloadString($"http://{url}/api/version");
                        settings.ApiServer.value = url;
                    }
                    catch (Exception)
                    {
                        ThreadHelper.dispatcher.Dispatch(() =>
                        {
                            MessagePanel panel = PanelManager.ShowPanel<MessagePanel>();
                            panel.DisplayInvalidApiServer();
                            if (urlInput)
                            {
                                urlInput.text = settings.ApiServer.value;
                            }
                        });
                    }

                }).Start();
            });
            urlInput.width = 400;

            UIHelperBase buttonsGroup = helper.AddGroup("Buttons");
            buttonsGroup.AddButton("Show Release Notes", () =>
            {
                MessagePanel panel = PanelManager.ShowPanel<MessagePanel>();
                panel.DisplayReleaseNotes();
            });

            buttonsGroup.AddButton("Check for updates", () =>
            {
                new Thread(() => MainMenuHandler.CheckForUpdate(true)).Start();
            });
        }
    }
}

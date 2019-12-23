using ColossalFramework.UI;
using ICities;

namespace CSM.Panels
{
    public static class SettingsPanel
    {
        public static void Build(UIHelperBase helper, Settings settings)
        {
            var advancedGroup = helper.AddGroup("Advanced");

            UICheckBox cb = (UICheckBox)advancedGroup.AddCheckbox("Enable debug logging (requires game restart)", settings.DebugLogging.value,
                c => { settings.DebugLogging.value = c; });
            cb.tooltip = "Note: This may cause excessive logging and slow down the game!";
        }
    }
}

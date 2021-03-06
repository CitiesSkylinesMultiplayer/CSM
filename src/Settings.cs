using ColossalFramework;
using CSM.Util;

namespace CSM
{
    public class Settings
    {
        public const string SettingsFile = "CitiesSkylinesMultiplayer";

        public Settings()
        {
            GameSettings.AddSettingsFile(new SettingsFile { fileName = SettingsFile });
        }

        private static readonly bool DefaultDebugLogging = false;
        private static readonly bool DefaultUseChirper = true;
        private static readonly bool DefaultPrintChirperMsgs = false;

        public readonly SavedBool DebugLogging =
            new SavedBool(nameof(DebugLogging), SettingsFile, DefaultDebugLogging, true);

        public bool UseChirper
        {
            get
            {
                if (ModCompat.HasDisableChirperMod)
                {
                    return false;
                }
                return _useChirper;
            }
            set => _useChirper.value = value;
        }

        private readonly SavedBool _useChirper =
            new SavedBool(nameof(UseChirper), SettingsFile, DefaultUseChirper, true);

        public readonly SavedBool PrintChirperMsgs =
            new SavedBool(nameof(PrintChirperMsgs), SettingsFile, DefaultPrintChirperMsgs, true);
    }
}

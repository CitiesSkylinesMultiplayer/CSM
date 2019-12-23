using ColossalFramework;

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

        public readonly SavedBool DebugLogging =
            new SavedBool(nameof(DebugLogging), SettingsFile, DefaultDebugLogging, true);
    }
}

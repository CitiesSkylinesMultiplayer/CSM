using ColossalFramework;
using CSM.Mods;

namespace CSM
{
    public class Settings
    {
        public const string SettingsFile = "CitiesSkylinesMultiplayer";

        public Settings()
        {
            GameSettings.AddSettingsFile(new SettingsFile { fileName = SettingsFile });
        }

        private const bool DefaultDebugLogging = false;
        private const bool DefaultUseChirper = true;
        private const bool DefaultPrintChirperMsgs = false;
        private const bool DefaultSkipModCompatibilityChecks = false;
        private const string DefaultLastSeenReleaseNotes = "0.0";
        private const string DefaultApiServer = "api.citiesskylinesmultiplayer.com";
        private const int DefaultApiServerPort = 4240;
        // LiteNetLib's built-in default; going lower risks dropping healthy peers between 1 s keepalive pings
        public const int MinConnectionTimeoutSeconds = 5;
        public const int MaxConnectionTimeoutSeconds = 600;

        public readonly SavedBool DebugLogging =
            new SavedBool(nameof(DebugLogging), SettingsFile, DefaultDebugLogging, true);

        public bool UseChirper
        {
            get => ModCompat.HasDisableChirperMod ? false : _useChirper;
            set => _useChirper.value = value;
        }

        private readonly SavedBool _useChirper =
            new SavedBool(nameof(UseChirper), SettingsFile, DefaultUseChirper, true);

        public readonly SavedBool PrintChirperMsgs =
            new SavedBool(nameof(PrintChirperMsgs), SettingsFile, DefaultPrintChirperMsgs, true);

        public readonly SavedString LastSeenReleaseNotes =
            new SavedString(nameof(LastSeenReleaseNotes), SettingsFile, DefaultLastSeenReleaseNotes, true);

        public readonly SavedBool SkipModCompatibilityChecks = 
            new SavedBool(nameof(SkipModCompatibilityChecks), SettingsFile, DefaultSkipModCompatibilityChecks, true);

        public readonly SavedString ApiServer =
            new SavedString(nameof(ApiServer), SettingsFile, DefaultApiServer, true);

        public readonly SavedInt ApiServerPort =
            new SavedInt(nameof(ApiServerPort), SettingsFile, DefaultApiServerPort, true);

        public readonly SavedInt ConnectionTimeoutSeconds =
            new SavedInt(nameof(ConnectionTimeoutSeconds), SettingsFile, MinConnectionTimeoutSeconds, true);

        // The settings file can be edited by hand, so the saved value is not trusted to be in range
        public int ConnectionTimeoutMs =>
            System.Math.Min(System.Math.Max(ConnectionTimeoutSeconds.value, MinConnectionTimeoutSeconds), MaxConnectionTimeoutSeconds) * 1000;
    }
}

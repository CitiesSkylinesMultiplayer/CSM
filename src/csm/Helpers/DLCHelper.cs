using CSM.Mods;

namespace CSM.Helpers
{
    public static class DLCHelper
    {
        public static SteamHelper.ExpansionBitMask GetOwnedExpansions()
        {
            // Ignore christmas as it is only music related, but not in the radio station bit mask :/
            return SteamHelper.GetOwnedExpansionMask() & ~SteamHelper.ExpansionBitMask.Christmas;
        }

        public static SteamHelper.ModderPackBitMask GetOwnedModderPacks()
        {
            return SteamHelper.GetOwnedModderPackMask();
        }

        public static DLCComparison Compare(SteamHelper.ExpansionBitMask serverExpansion, SteamHelper.ExpansionBitMask clientExpansion, SteamHelper.ModderPackBitMask serverModderPack, SteamHelper.ModderPackBitMask clientModderPack)
        {
            return new DLCComparison
            {
                ServerMissingExpansions = ~serverExpansion & clientExpansion,
                ClientMissingExpansions = serverExpansion & ~clientExpansion,
                ServerMissingModderPack = ~serverModderPack & clientModderPack,
                ClientMissingModderPack = serverModderPack & ~clientModderPack,
            };
        }

        public static string GetDlcName(SteamHelper.DLC dlc)
        {
            DLCList dlcPanel = UnityEngine.Object.FindObjectOfType<DLCList>();
            return GetDlcName(dlcPanel, dlc);
        }

        public static string GetDlcName(DLCList list, SteamHelper.DLC dlc)
        {
            string dlcName = list.FindLocalizedDLCName(dlc);
            if (string.IsNullOrEmpty(dlcName))
            {
                // Default to enum item name
                dlcName = dlc.ToString();
            }

            return dlcName;
        }

        public static ModSupportType GetSupport(SteamHelper.DLC dlc)
        {
            switch (dlc)
            {
                // DLCs
                case SteamHelper.DLC.AfterDarkDLC:
                    return ModSupportType.Unknown;
                case SteamHelper.DLC.AirportDLC:
                    return ModSupportType.Unknown;
                case SteamHelper.DLC.CampusDLC:
                    return ModSupportType.Supported;
                case SteamHelper.DLC.MusicFestival: // Concerts
                    return ModSupportType.Unknown;
                case SteamHelper.DLC.FinancialDistrictsDLC:
                    return ModSupportType.Unknown;
                case SteamHelper.DLC.GreenCitiesDLC:
                    return ModSupportType.Unknown;
                case SteamHelper.DLC.HotelDLC:
                    return ModSupportType.Unknown;
                case SteamHelper.DLC.IndustryDLC:
                    return ModSupportType.Unknown;
                case SteamHelper.DLC.InMotionDLC: // Mass transit
                    return ModSupportType.Unknown;
                case SteamHelper.DLC.Football: // Match day
                    return ModSupportType.Supported;
                case SteamHelper.DLC.NaturalDisastersDLC:
                    return ModSupportType.Unsupported;
                case SteamHelper.DLC.ParksDLC:
                    return ModSupportType.Unknown;
                case SteamHelper.DLC.PlazasAndPromenadesDLC:
                    return ModSupportType.Unknown;
                case SteamHelper.DLC.SnowFallDLC:
                    return ModSupportType.Supported;
                case SteamHelper.DLC.UrbanDLC: // Sunset Harbor
                    return ModSupportType.Unknown;

                // Cosmetics
                case SteamHelper.DLC.DeluxeDLC:
                case SteamHelper.DLC.Football2345:
                case SteamHelper.DLC.ModderPack1:
                case SteamHelper.DLC.ModderPack2:
                case SteamHelper.DLC.ModderPack3:
                case SteamHelper.DLC.ModderPack4:
                case SteamHelper.DLC.ModderPack5:
                case SteamHelper.DLC.ModderPack6:
                case SteamHelper.DLC.ModderPack7:
                case SteamHelper.DLC.ModderPack8:
                case SteamHelper.DLC.ModderPack9:
                case SteamHelper.DLC.ModderPack10:
                case SteamHelper.DLC.ModderPack11:
                case SteamHelper.DLC.ModderPack12:
                case SteamHelper.DLC.ModderPack13:
                case SteamHelper.DLC.ModderPack14:
                case SteamHelper.DLC.ModderPack15:
                case SteamHelper.DLC.ModderPack16:
                case SteamHelper.DLC.ModderPack17:
                case SteamHelper.DLC.ModderPack18:
                case SteamHelper.DLC.ModderPack19:
                case SteamHelper.DLC.ModderPack20:
                case SteamHelper.DLC.ModderPack21:
                case SteamHelper.DLC.OrientalBuildings:
                    return ModSupportType.Supported;
                default:
                    return ModSupportType.Unknown;
            }
        }

        public class DLCComparison
        {
            public SteamHelper.ExpansionBitMask ServerMissingExpansions { get; set; }
            public SteamHelper.ExpansionBitMask ClientMissingExpansions { get; set; }
            public SteamHelper.ModderPackBitMask ServerMissingModderPack { get; set; }
            public SteamHelper.ModderPackBitMask ClientMissingModderPack { get; set; }
        }
    }
}

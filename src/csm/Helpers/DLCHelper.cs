namespace CSM.Helpers
{
    public static class DLCHelper
    {
        public static SteamHelper.ExpansionBitMask GetOwnedExpansions()
        {
            return SteamHelper.GetOwnedExpansionMask();
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

        public class DLCComparison
        {
            public SteamHelper.ExpansionBitMask ServerMissingExpansions { get; set; }
            public SteamHelper.ExpansionBitMask ClientMissingExpansions { get; set; }
            public SteamHelper.ModderPackBitMask ServerMissingModderPack { get; set; }
            public SteamHelper.ModderPackBitMask ClientMissingModderPack { get; set; }
        }
    }
}

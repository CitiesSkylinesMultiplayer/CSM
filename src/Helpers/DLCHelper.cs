namespace CSM.Helpers
{
    public static class DLCHelper
    {
        private const SteamHelper.DLC_BitMask RadioStations = SteamHelper.DLC_BitMask.RadioStation1 |
                                                              SteamHelper.DLC_BitMask.RadioStation2 |
                                                              SteamHelper.DLC_BitMask.RadioStation3 |
                                                              SteamHelper.DLC_BitMask.RadioStation4 |
                                                              SteamHelper.DLC_BitMask.RadioStation5 |
                                                              SteamHelper.DLC_BitMask.RadioStation6 |
                                                              SteamHelper.DLC_BitMask.RadioStation7;

        private static SteamHelper.DLC_BitMask RemoveRadioStations(SteamHelper.DLC_BitMask bitmask)
        {
            return bitmask & ~RadioStations;
        }

        public static SteamHelper.DLC_BitMask GetOwnedDLCs()
        {
            return RemoveRadioStations(SteamHelper.GetOwnedDLCMask());
        }

        public static DLCComparison Compare(SteamHelper.DLC_BitMask server, SteamHelper.DLC_BitMask client)
        {
            return new DLCComparison
            {
                ServerMissing = ~server & client,
                ClientMissing = server & ~client,
            };
        }

        public class DLCComparison
        {
            public SteamHelper.DLC_BitMask ServerMissing { get; set; }
            public SteamHelper.DLC_BitMask ClientMissing { get; set; }
        }
    }
}

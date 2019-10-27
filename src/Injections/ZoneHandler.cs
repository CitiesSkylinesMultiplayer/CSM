using CSM.Commands;
using CSM.Commands.Data.Zones;
using CSM.Helpers;
using HarmonyLib;

namespace CSM.Injections
{
    /*
     * Notes:
     * - ZoneBlocks are created/destroyed by node segments, so we don't need to sync that
     * - We also don't need to sync the id, because the generated id is deterministic (the seed for the randomizer is the segment id)
     */

    [HarmonyPatch(typeof(ZoneBlock))]
    [HarmonyPatch("RefreshZoning")]
    public class RefreshZoning
    {
        /// <summary>
        ///     This method is executed after ZoneBlock::RefreshZoning is called.
        ///     RefreshZoning is called after the player changed any of the zones in the block.
        /// </summary>
        /// <param name="blockID">The id of the modified block.</param>
        /// <param name="___m_zone1">Zone storage attribute 1 (three underscores to access an attribute of the class)</param>
        /// <param name="___m_zone2">Zone storage attribute 2</param>
        public static void Postfix(ushort blockID, ulong ___m_zone1, ulong ___m_zone2)
        {
            if (IgnoreHelper.IsIgnored())
                return;

            Command.SendToAll(new ZoneUpdateCommand
            {
                ZoneId = blockID,
                Zone1 = ___m_zone1,
                Zone2 = ___m_zone2
            });
        }
    }
}

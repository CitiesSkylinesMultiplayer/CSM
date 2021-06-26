using HarmonyLib;
using NLog;
using SampleExternalMod;
using SampleExternalMod.Commands;

namespace CSM.Injections
{
    [HarmonyPatch(typeof(RoadBaseAI))]
    [HarmonyPatch("ClickNodeButton")]
    public class ClickNodeButton
    {
        private static readonly Logger _logger = LogManager.GetLogger("CSM");

        public static void Postfix(ref NetNode data, int index, ushort nodeID, ref int __state)
        {
            _logger.Info("Doing something");
            CSMConnection.Instance.SentToAll(new TestCommand());
        }
    }
}
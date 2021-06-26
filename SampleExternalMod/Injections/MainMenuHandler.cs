using CSM.Helpers;
using HarmonyLib;
using NLog;
using SampleExternalMod.Commands;

namespace SampleExternalMod.Injections
{
    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("CreateDistrict")]
    public class CreateDistrict
    {
        private static readonly Logger _logger = LogManager.GetLogger("CSM");

        public static void Postfix(bool __result, ref byte district)
        {
            _logger.Info("Doing something");
            // CSMConnection.Instance.SentToAll(new TestCommand()
            // {
            //     testing = "Heyyyy"
            // });
            _logger.Info(IgnoreHelper.Instance.IsIgnored());
        }
    }
}
using CSM.API.Commands;
using CSM.API.Helpers;
using HarmonyLib;
using SampleExternalMod.Commands.Data;

namespace SampleExternalMod.Injections
{
    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("CreateDistrict")]
    public class CreateDistrict
    {
        public static void Postfix(bool __result, ref byte district)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            Command.SendToAll(new TestCommand()
            {
                testing = "Heyyyy"
            });
        }
    }
}

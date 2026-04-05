using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Disasters;
using HarmonyLib;

namespace CSM.BaseGame.Injections.Buildings
{
    [HarmonyPatch(typeof(CommonBuildingAI))]
    [HarmonyPatch("SetEvacuating")]
    public class BuildingAIHandler
    {
        public static void Prefix(ushort buildingID, ref Building data, bool evacuating)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            if (Command.CurrentRole == MultiplayerRole.None)
                return;

            bool isEvacuating = (data.m_flags & Building.Flags.Evacuating) != Building.Flags.None;
            if (isEvacuating == evacuating)
                return;

            Command.SendToAll(new EvacuationCommand
            {
                BuildingID = buildingID,
                Evacuating = evacuating
            });
        }
    }
}

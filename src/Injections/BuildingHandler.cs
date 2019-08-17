using CSM.Commands;
using Harmony;

namespace CSM.Injections
{
    public static class BuildingHandler
    {
        public static bool IgnoreAll { get; set; } = false;
    }

    [HarmonyPatch(typeof(BuildingManager))]
    [HarmonyPatch("CreateBuilding")]
    public class CreateBuilding
    {
        public static void Postfix(bool __result, ref ushort building)
        {
            if (BuildingHandler.IgnoreAll)
                return;

            if (__result)
            {
                Building b = BuildingManager.instance.m_buildings.m_buffer[building];

                Command.SendToAll(new BuildingCreateCommand
                {
                    BuildingId = building,
                    Position = b.m_position,
                    InfoIndex = b.m_infoIndex,
                    Angle = b.m_angle,
                    Length = b.Length
                });
            }
        }
    }

    [HarmonyPatch(typeof(BuildingManager))]
    [HarmonyPatch("ReleaseBuildingImplementation")]
    public class ReleaseBuildingImplementation
    {
        public static void Postfix(ushort building)
        {
            if (BuildingHandler.IgnoreAll)
                return;

            Command.SendToAll(new BuildingRemoveCommand
            {
                BuildingId = building
            });
        }
    }

    [HarmonyPatch(typeof(BuildingManager))]
    [HarmonyPatch("RelocateBuildingImpl")]
    public class RelocateBuildingImpl
    {
        public static void Postfix(ushort building)
        {
            if (BuildingHandler.IgnoreAll)
                return;

            Building b = BuildingManager.instance.m_buildings.m_buffer[building];

            Command.SendToAll(new BuildingRelocateCommand
            {
                BuildingId = building,
                NewPosition = b.m_position,
                Angle = b.m_angle
            });
        }
    }
}

using CSM.Commands;
using CSM.Models;
using CSM.Networking;
using Harmony;
using ICities;
using UnityEngine;

namespace CSM.Injections
{
    public class BuildingHandler
    {
        /// <summary>
        ///     This handles Creating, Releasing and Relocating buildings.
        ///     When a building is created it generate a random BuildingID which is it's placement in the m_buffer[] array since it is random this code relies on it being shared between the server and clients
        ///     On initialization all existing initialized buildings are added to the dictionary, this takes advantage of the fact that copies of the same safe game, have the same BuildingID.
        ///     but it also makes it a requirement that both server and clients starts from exactly the same save game, which means that the save game will have to be re-shared each time the game is loading
        /// </summary>

        public static bool HoldAll { get; set; } = false;
        public static FastList<CommandBase> queuedCommands = new FastList<CommandBase>();

        public static Vector3 LastPosition { get; set; }
        public static uint lastRelease;

    }

    [HarmonyPatch(typeof(BuildingManager))]
    [HarmonyPatch("CreateBuilding")]
    public class CreateBuilding
    {
        public static bool Prefix()
        {
            if (MultiplayerManager.Instance.CurrentRole.Equals(MultiplayerRole.Client))
            {
                return !BuildingHandler.HoldAll;
            } else {
                return true;
            }
        }

        public static void Postfix(ref ushort building)
        {
            ushort id = building;
            if (id != 0)
            {
                var Instance = BuildingManager.instance;
                var position = Instance.m_buildings.m_buffer[id].m_position;  //the building data is stored in Instance.m_buildings.m_buffer[]
                var angle = Instance.m_buildings.m_buffer[id].m_angle;
                var length = Instance.m_buildings.m_buffer[id].Length;
                var infoindex = Instance.m_buildings.m_buffer[id].m_infoIndex; //by sending the info index, the receiver can generate Building_info from the prefab
                if (BuildingHandler.LastPosition != position)
                {
                    CommandBase cmd = new BuildingCreateCommand
                    {
                        BuildingID = id,
                        Position = position,
                        Infoindex = infoindex,
                        Angle = angle,
                        Length = length,
                    };
                    if (BuildingHandler.HoldAll)
                    {
                        BuildingHandler.queuedCommands.Add(cmd);
                    }
                    else
                    {
                        Command.SendToAll(cmd);
                    }
                }

                BuildingHandler.LastPosition = position;
            }
        }
    }

    [HarmonyPatch(typeof(BuildingManager))]
    [HarmonyPatch("ReleaseBuildingImplementation")]
    public class ReleaseBuildingImplementation
    {
        public static void Postfix(ushort building)
        {
            ushort id = building;
            if (BuildingHandler.lastRelease != id)
            {
                Command.SendToAll(new BuildingRemoveCommand
                {
                    BuildingId = id
                });
            }
            BuildingHandler.lastRelease = id;
        }
    }

    [HarmonyPatch(typeof(BuildingManager))]
    [HarmonyPatch("RelocateBuildingImpl")]
    public class RelocateBuildingImpl
    {
        public static void Postfix(ushort building)
        {
            ushort id = building;
            var newPosition = BuildingManager.instance.m_buildings.m_buffer[id].m_position;
            var angle = BuildingManager.instance.m_buildings.m_buffer[id].m_angle;
            if (BuildingHandler.LastPosition != newPosition)
            {
                Command.SendToAll(new BuildingRelocateCommand
                {
                    BuidlingId = id,
                    NewPosition = newPosition,
                    Angle = angle,
                });
            }
            BuildingHandler.LastPosition = newPosition;
        }
    }
}
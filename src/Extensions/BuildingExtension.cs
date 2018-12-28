using CSM.Commands;
using CSM.Models;
using ICities;
using UnityEngine;

namespace CSM.Extensions
{
    public class BuildingExtension : BuildingExtensionBase
    {
        /// <summary>
        ///     This handles Creating, Releasing and Relocating buildings.
        ///     When a building is created it generate a random BuildingID which is it's placement in the m_buffer[] array since it is random this code relies on it being shared between the server and clients
        ///     On initialization all existing initialized buildings are added to the dictionary, this takes advantage of the fact that copies of the same safe game, have the same BuildingID.
        ///     but it also makes it a requirement that both server and clients starts from exactly the same save game, which means that the save game will have to be re-shared each time the game is loading
        /// </summary>

        public static Vector3 LastPosition { get; set; }
        public static uint lastRelease;

        public override void OnBuildingCreated(ushort id)
        {
            base.OnBuildingCreated(id);
            var Instance = BuildingManager.instance;
            var position = Instance.m_buildings.m_buffer[id].m_position;  //the building data is stored in Instance.m_buildings.m_buffer[]
            var angle = Instance.m_buildings.m_buffer[id].m_angle;
            var length = Instance.m_buildings.m_buffer[id].Length;
            var infoindex = Instance.m_buildings.m_buffer[id].m_infoIndex; //by sending the info index, the receiver can generate Building_info from the prefab
            if (LastPosition != position)
            {
                Command.SendToAll(new BuildingCreateCommand
                {
                    BuildingID = id,
                    Position = position,
                    Infoindex = infoindex,
                    Angle = angle,
                    Length = length,
                });
            }

            LastPosition = position;
        }

        public override void OnBuildingReleased(ushort id)
        {
            base.OnBuildingReleased(id);

            if (lastRelease != id)
            {
                Command.SendToAll(new BuildingRemoveCommand
                {
                    BuildingId = id
                });
            }
            lastRelease = id;
        }

        public override void OnBuildingRelocated(ushort id)
        {
            base.OnBuildingRelocated(id);
            var newPosition = BuildingManager.instance.m_buildings.m_buffer[id].m_position;
            var angle = BuildingManager.instance.m_buildings.m_buffer[id].m_angle;
            if (LastPosition != newPosition)
            {
                Command.SendToAll(new BuildingRelocateCommand
                {
                    BuidlingId = id,
                    NewPosition = newPosition,
                    Angle = angle,
                });
            }
            LastPosition = newPosition;
        }
    }
}
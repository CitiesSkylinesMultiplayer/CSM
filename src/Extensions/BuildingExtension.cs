using CSM.Commands;
using CSM.Helpers;
using ICities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CSM.Extensions
{
    public class BuildingExtension : BuildingExtensionBase
    {
        /// <summary>
        ///     This handles Creating, Releasing and Relocating buildings.
        ///     When a building is created it generate a random BuildingID which is it's placement in the m_buffer[] array since it is random this code relies on it being shared between the server and clients
        ///     On initialisation all existing initialized buildings are added to the dictionary, this takes advantage of the fact that copies of the same safe game, have the same BuildingID.
        ///     but it also makes it a requrement that both server and clients starts from exactely the same save game, which means that the save game will have to be reshared each time the game is loading
        /// </summary>

        public static Vector3 LastPosition { get; set; }
        public static Dictionary<uint, uint> BuildingID = new Dictionary<uint, uint>();
        public static ushort lastRelease;
        public static ushort lastRelocation;

        private Vector3 _nonvector = new Vector3(0.0f, 0.0f, 0.0f);

        public override void OnCreated(IBuilding building)
        {
            if (!ProtoBuf.Meta.RuntimeTypeModel.Default.IsDefined(typeof(Vector3)))
            {
                ProtoBuf.Meta.RuntimeTypeModel.Default[typeof(Vector3)].SetSurrogate(typeof(Vector3Surrogate));
            }

            //  Since the dictionary is lost when the program is terminated, it has to be recreated at startup this adds all buildings initialised in the m_buffer to the dictionary

            for (int i = 0; i < BuildingManager.instance.m_buildings.m_buffer.Length; i++)
            {
                if (BuildingManager.instance.m_buildings.m_buffer[i].m_position != _nonvector)
                {
                    BuildingID.Add((ushort)i, (ushort)i);
                }
            }
        }

        public override void OnBuildingCreated(ushort id)
        {
            base.OnBuildingCreated(id);
            var Instance = BuildingManager.instance;
            var position = Instance.m_buildings.m_buffer[id].m_position;  //the building data is stored in Instance.m_buildings.m_buffer[]
            var angle = Instance.m_buildings.m_buffer[id].m_angle;
            var length = Instance.m_buildings.m_buffer[id].Length;
            var infoindex = Instance.m_buildings.m_buffer[id].m_infoIndex; //by sending the infoindex, the reciever can generate Building_info from the prefap
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
            //UnityEngine.Debug.Log($"Building Created ID {id}");
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

                foreach (var ID in BuildingID.Where(kvp => kvp.Value == id).ToList())
                {
                    BuildingID.Remove(ID.Key);
                }
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
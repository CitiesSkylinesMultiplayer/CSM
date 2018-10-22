using ICities;
using CSM.Networking;
using CSM.Commands;
using UnityEngine;
using CSM.Helpers;
using ColossalFramework;
using System.Linq;

namespace CSM.Extensions
{

    public class BuildingExtension : BuildingExtensionBase
    {
		public static Vector3 LastPosition{ get; set; }


		public override void OnCreated(IBuilding building)
        {
			
			if (!ProtoBuf.Meta.RuntimeTypeModel.Default.IsDefined(typeof(Vector3)))
			{
				ProtoBuf.Meta.RuntimeTypeModel.Default[typeof(Vector3)].SetSurrogate(typeof(Vector3Surrogate));
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
				switch (MultiplayerManager.Instance.CurrentRole)
				{
					case MultiplayerRole.Server:
						MultiplayerManager.Instance.CurrentServer.SendToClients(CommandBase.CreatedCommandID, new BuildingCreatedCommand
						{
							BuildingID = id,
							Position = position,
							Infoindex = infoindex,
							Angel = angle,
							Length = length,
						});
						break;

					case MultiplayerRole.Client:
						MultiplayerManager.Instance.CurrentClient.SendToServer(CommandBase.CreatedCommandID, new BuildingCreatedCommand
						{
							BuildingID = id,
							Position = position,
							Infoindex = infoindex,
							Angel = angle,
							Length = length,
						});
						break;
				}
			}
			LastPosition = position;

		}

		public override void OnBuildingReleased(ushort id)
        {
            base.OnBuildingReleased(id);
			var position = BuildingManager.instance.m_buildings.m_buffer[id].m_position; //Sending the position of the deleted building is nessesary to calculate the index in M_buildinggrid[index] and get the BuildingID


			switch (MultiplayerManager.Instance.CurrentRole)
			{
				case MultiplayerRole.Server:
					MultiplayerManager.Instance.CurrentServer.SendToClients(CommandBase.BuildingRemovedCommandID, new BuildingRemovedCommand
					{
						position = position,
					});
					break;

				case MultiplayerRole.Client:
					MultiplayerManager.Instance.CurrentClient.SendToServer(CommandBase.BuildingRemovedCommandID, new BuildingRemovedCommand
					{
						position = position,
					});
					break;
			}
			
		}

        public override void OnBuildingRelocated(ushort id)
        {
            base.OnBuildingRelocated(id);
        }
    }
}
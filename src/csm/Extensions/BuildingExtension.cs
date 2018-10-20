using ICities;
using CSM.Networking;
using CSM.Commands;
using UnityEngine;
using CSM.Helpers;
using ColossalFramework;

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
			var position = Instance.m_buildings.m_buffer[id].m_position;
			var angle = Instance.m_buildings.m_buffer[id].m_angle;
			var length = Instance.m_buildings.m_buffer[id].Length;
			var infoindex = Instance.m_buildings.m_buffer[id].m_infoIndex;

			if (LastPosition != position)
			{
			switch (MultiplayerManager.Instance.CurrentRole)
				{
				case MultiplayerRole.Server:
					MultiplayerManager.Instance.CurrentServer.SendToClients(CommandBase. CreatedCommandID, new BuildingCreatedCommand
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
        }

        public override void OnBuildingRelocated(ushort id)
        {
            base.OnBuildingRelocated(id);
        }
    }
}
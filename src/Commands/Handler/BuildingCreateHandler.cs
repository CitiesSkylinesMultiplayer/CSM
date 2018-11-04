using ColossalFramework;
using CSM.Extensions;
using CSM.Networking;


namespace CSM.Commands.Handler
{
	public class BuildingCreateHandler : CommandHandler<BuildingCreateCommand>
	{
		public override byte ID => 103;

		public override void HandleOnServer(BuildingCreateCommand command, Player player) => HandleBuilding(command);

		public override void HandleOnClient(BuildingCreateCommand command) => HandleBuilding(command);

		private void HandleBuilding(BuildingCreateCommand command)
		{
			BuildingInfo info = PrefabCollection<BuildingInfo>.GetPrefab(command.Infoindex);
			BuildingExtension.LastPosition = command.Position;
			Singleton<BuildingManager>.instance.CreateBuilding(out ushort building, ref Singleton<SimulationManager>.instance.m_randomizer, info, command.Position, command.Angle, command.Length, Singleton<SimulationManager>.instance.m_currentBuildIndex);
			Extensions.BuildingExtension.BuildingID.Add(command.BuildingID, building);

			switch (MultiplayerManager.Instance.CurrentRole) //returns the newly created buildings buildingID so that it can be added to the original builders Dictionary
			{
				case MultiplayerRole.Client:
					{
						Command.SendToServer(new BuildingIDCommand
						{
							BuildingIDSender = building,
							BuildingIDReciever = command.BuildingID
						});


						break;
					}
				case MultiplayerRole.Server:
					{
						Command.SendToServer(new BuildingIDCommand
						{
							BuildingIDSender = building,
							BuildingIDReciever = command.BuildingID

						});
						break;
					}



			}

		}
	}

}
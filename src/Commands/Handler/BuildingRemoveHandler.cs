using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class BuildingRemoveHandler : CommandHandler<BuildingRemoveCommand>
    {
        public override byte ID => 104;

        public override void HandleOnServer(BuildingRemoveCommand command, Player player) => HandleBuilding(command);

        public override void HandleOnClient(BuildingRemoveCommand command) => HandleBuilding(command);

        private void HandleBuilding(BuildingRemoveCommand command)
        {
            uint buildingID = Extensions.BuildingExtension.BuildingID[command.BuildingID];
            Extensions.BuildingExtension.lastRelease = (ushort)buildingID;
            BuildingManager.instance.ReleaseBuilding((ushort)buildingID);
        }
    }
}
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class BuildingRemoveHandler : CommandHandler<BuildingRemoveCommand>
    {
        public override byte ID => CommandIds.BuildingRemoveCommand;

        public override void HandleOnServer(BuildingRemoveCommand command, Player player) => HandleBuilding(command);

        public override void HandleOnClient(BuildingRemoveCommand command) => HandleBuilding(command);

        private void HandleBuilding(BuildingRemoveCommand command)
        {
            Extensions.BuildingExtension.lastRelease = command.BuildingId;
            BuildingManager.instance.ReleaseBuilding((ushort)command.BuildingId);
        }
    }
}
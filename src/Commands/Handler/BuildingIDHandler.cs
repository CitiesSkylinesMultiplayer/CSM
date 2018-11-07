using CSM.Networking;

namespace CSM.Commands.Handler
{
    /// <summary>
    ///     When a building is created it generates a random BuildingID,
    ///     This returns the BuildingID of the receiver of a BuildingCommand, and adds it to the original senders BuildingID Dictionary
    /// </summary>
    public class BuildingIdHandler : CommandHandler<BuildingIdCommand>
    {
        public override byte ID => 111;

        public override void HandleOnServer(BuildingIdCommand command, Player player) => HandleBuilding(command);

        public override void HandleOnClient(BuildingIdCommand command) => HandleBuilding(command);

        private void HandleBuilding(BuildingIdCommand command)
        {
            Extensions.BuildingExtension.BuildingID.Add(command.BuildingIdSender, command.BuildingIdReciever);
        }
    }
}
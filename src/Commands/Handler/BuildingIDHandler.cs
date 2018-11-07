using CSM.Networking;

namespace CSM.Commands.Handler
{
    /// <summary>
    /// When a building is created it generates a random BuildingID,
    /// This returns the BuildingID of the reciever of a BuildingCommand, and adds it to the original senders BuildingID Dictionary
    /// </summary>
    internal class BuildingIDHandler : CommandHandler<BuildingIDCommand>
    {
        public override byte ID => 111;

        public override void HandleOnServer(BuildingIDCommand command, Player player) => HandleBuilding(command);

        public override void HandleOnClient(BuildingIDCommand command) => HandleBuilding(command);

        private void HandleBuilding(BuildingIDCommand command)
        {
            Extensions.BuildingExtension.BuildingID.Add(command.BuildingIDSender, command.BuildingIDReciever);
        }
    }
}
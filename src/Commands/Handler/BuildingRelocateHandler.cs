using ColossalFramework;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class BuildingRelocateHandler : CommandHandler<BuildingRelocateCommand>
    {
        public override byte ID => CommandIds.BuildingRelocateCommand;

        public override void HandleOnServer(BuildingRelocateCommand command, Player player) => HandleBuilding(command);

        public override void HandleOnClient(BuildingRelocateCommand command) => HandleBuilding(command);

        private void HandleBuilding(BuildingRelocateCommand command)
        {
            Singleton<BuildingManager>.instance.RelocateBuilding((ushort)command.BuidlingId, command.NewPosition, command.Angle);
        }
    }
}
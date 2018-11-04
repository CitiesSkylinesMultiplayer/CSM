using ColossalFramework;
using CSM.Networking;
using UnityEngine;

namespace CSM.Commands.Handler
{
    public class BuildingRelocateHandler : CommandHandler<BuildingRelocateCommand>
    {
        public override byte ID => 105;

        public override void HandleOnServer(BuildingRelocateCommand command, Player player) => HandleBuilding(command);

        public override void HandleOnClient(BuildingRelocateCommand command) => HandleBuilding(command);

        private void HandleBuilding(BuildingRelocateCommand command)
        {
            uint BuildingId = Extensions.BuildingExtension.BuildingID[command.BuidlingId];
			Extensions.BuildingExtension.lastRelocation = (ushort)BuildingId;
            Singleton<BuildingManager>.instance.RelocateBuilding((ushort)BuildingId, command.NewPosition, command.Angle);
        }
    }
}
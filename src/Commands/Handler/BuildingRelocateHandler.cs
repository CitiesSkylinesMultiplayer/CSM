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
            long num = Mathf.Clamp((int)((command.OldPosition.x / 64f) + 135f), 0, 0x10d); //The buildingID is stored in the M_buildingGrid[index] which is calculated by thís arbitrary calculation using the buildings position
            long index = (Mathf.Clamp((int)((command.OldPosition.z / 64f) + 135f), 0, 0x10d) * 270) + num;
            ushort BuildingId = BuildingManager.instance.m_buildingGrid[index];
            Singleton<BuildingManager>.instance.RelocateBuilding(BuildingId, command.NewPosition, command.Angle);
        }
    }
}
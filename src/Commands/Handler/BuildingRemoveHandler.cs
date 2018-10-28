
using CSM.Networking;
using UnityEngine;

namespace CSM.Commands.Handler
{
    class BuildingRemoveHandler : CommandHandler<BuildingRemoveCommand>
    {
        public override byte ID => 104;

        public override void HandleOnServer(BuildingRemoveCommand command, Player player) => HandleBuilding(command);

        public override void HandleOnClient(BuildingRemoveCommand command) => HandleBuilding(command);

        private void HandleBuilding(BuildingRemoveCommand command)
        {
            long num = Mathf.Clamp((int) ((command.Position.x / 64f) + 135f), 0, 0x10d); // The buildingID is stored in the M_buildingGrid[index] which is calculated by this arbitrary calculation using the buildings position
            long index = (Mathf.Clamp((int) ((command.Position.z / 64f) + 135f), 0, 0x10d) * 270) + num;
            var BuildingId = BuildingManager.instance.m_buildingGrid[index];
            if (BuildingId != 0)
            {
                BuildingManager.instance.ReleaseBuilding(BuildingId);
            }
        }
    }
}

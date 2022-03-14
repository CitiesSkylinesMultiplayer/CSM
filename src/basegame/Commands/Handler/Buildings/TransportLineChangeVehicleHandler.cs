using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Buildings;

namespace CSM.BaseGame.Commands.Handler.Buildings
{
    public class ServiceBuildingChangeVehicleHandler : CommandHandler<ServiceBuildingChangeVehicleCommand>
    {
        protected override void Handle(ServiceBuildingChangeVehicleCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            PlayerBuildingAI pbi = BuildingManager.instance.m_buildings.m_buffer[command.BuildingId].Info.m_buildingAI as PlayerBuildingAI;

            VehicleInfo vehicle = null;
            if (command.Vehicle.HasValue) {
                vehicle = PrefabCollection<VehicleInfo>.GetPrefab(command.Vehicle.Value);
            }

            ReflectionHelper.Call(pbi, "ReplaceVehicles", new [] { typeof(ushort), typeof(VehicleInfo) }, command.BuildingId, vehicle);

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

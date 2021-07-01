using ColossalFramework.UI;
using CSM.API.Commands;
using CSM.Commands.Data.Buildings;
using CSM.Helpers;

namespace CSM.Commands.Handler.Buildings
{
    public class BuildingSetHistoricalHandler : CommandHandler<BuildingSetHistoricalCommand>
    {
        protected override void Handle(BuildingSetHistoricalCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            ref Building building = ref BuildingManager.instance.m_buildings.m_buffer[command.Building];
            building.Info.m_buildingAI.SetHistorical(command.Building, ref building, command.Historical);

            if (InfoPanelHelper.IsBuilding(typeof(ZonedBuildingWorldInfoPanel), command.Building, out WorldInfoPanel panel))
            {
                UICheckBox isHistorical = ReflectionHelper.GetAttr<UICheckBox>(panel, "m_IsHistorical");
                SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                {
                    isHistorical.isChecked = command.Historical;
                });
            }

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

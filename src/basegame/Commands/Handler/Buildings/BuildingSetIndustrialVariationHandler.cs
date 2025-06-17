using ColossalFramework;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Buildings;
using CSM.BaseGame.Helpers;

namespace CSM.BaseGame.Commands.Handler.Buildings
{
    public class BuildingSetIndustrialVariationHandler : CommandHandler<BuildingSetIndustrialVariationCommand>
    {
        protected override void Handle(BuildingSetIndustrialVariationCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            BuildingInfo newPrefab = PrefabCollection<BuildingInfo>.GetPrefab(command.VariationInfoIndex);
            Singleton<BuildingManager>.instance.UpdateBuildingInfo(command.Building, newPrefab);

            ((IndustryBuildingAI) Singleton<BuildingManager>.instance.m_buildings.m_buffer[command.Building].Info.m_buildingAI).SetLastVariationIndex(command.VariationIndex);

            // Refresh panel
            if (InfoPanelHelper.IsBuilding(typeof(CityServiceWorldInfoPanel), command.Building,
                    out WorldInfoPanel panel))
            {
                SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                {
                    ReflectionHelper.Call((CityServiceWorldInfoPanel)panel, "OnSetTarget");
                });
            }

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

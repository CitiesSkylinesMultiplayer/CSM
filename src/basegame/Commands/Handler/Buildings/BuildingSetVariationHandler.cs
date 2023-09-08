using ColossalFramework;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Buildings;

namespace CSM.BaseGame.Commands.Handler.Buildings
{
    public class BuildingSetVariationHandler : CommandHandler<BuildingSetVariationCommand>
    {
        protected override void Handle(BuildingSetVariationCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            CommonBuildingAI building_ai = Singleton<BuildingManager>.instance.m_buildings.m_buffer[command.Building].Info.m_buildingAI as CommonBuildingAI;
            if (building_ai != null)
                building_ai.ReplaceVariation(command.Building, command.Variation);

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

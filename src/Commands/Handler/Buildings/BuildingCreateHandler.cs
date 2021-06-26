using CSM.API.Commands;
using CSM.Commands.Data.Buildings;
using CSM.Helpers;
using CSM.Injections;

namespace CSM.Commands.Handler.Buildings
{
    public class BuildingCreateHandler : CommandHandler<BuildingCreateCommand>
    {
        protected override void Handle(BuildingCreateCommand command)
        {
            BuildingInfo info = PrefabCollection<BuildingInfo>.GetPrefab(command.InfoIndex);

            IgnoreHelper.Instance.StartIgnore();
            ArrayHandler.StartApplying(command.Array16Ids, command.Array32Ids);

            BuildingManager.instance.CreateBuilding(out _, ref SimulationManager.instance.m_randomizer, info,
                command.Position, command.Angle, command.Length, SimulationManager.instance.m_currentBuildIndex++);

            ArrayHandler.StopApplying();
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

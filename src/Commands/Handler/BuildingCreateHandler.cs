using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class BuildingCreateHandler : CommandHandler<BuildingCreateCommand>
    {
        public override void Handle(BuildingCreateCommand command)
        {
            BuildingInfo info = PrefabCollection<BuildingInfo>.GetPrefab(command.InfoIndex);

            NetHandler.IgnoreAll = true;
            TreeHandler.IgnoreAll = true;
            PropHandler.IgnoreAll = true;
            BuildingHandler.IgnoreAll = true;
            ArrayHandler.StartApplying(command.Array16Ids, command.Array32Ids);

            BuildingManager.instance.CreateBuilding(out _, ref SimulationManager.instance.m_randomizer, info,
                command.Position, command.Angle, command.Length, SimulationManager.instance.m_currentBuildIndex++);

            ArrayHandler.StopApplying();
            BuildingHandler.IgnoreAll = false;
            PropHandler.IgnoreAll = false;
            TreeHandler.IgnoreAll = false;
            NetHandler.IgnoreAll = false;
        }
    }
}

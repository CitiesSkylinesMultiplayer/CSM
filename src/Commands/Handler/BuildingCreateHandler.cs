using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class BuildingCreateHandler : CommandHandler<BuildingCreateCommand>
    {
        public override void Handle(BuildingCreateCommand command)
        {
            BuildingInfo info = PrefabCollection<BuildingInfo>.GetPrefab(command.InfoIndex);

            BuildingHandler.IgnoreAll = true;
            ArrayHandler.StartApplying(new ushort[] {command.BuildingId}, null);

            BuildingManager.instance.CreateBuilding(out _, ref SimulationManager.instance.m_randomizer, info,
                command.Position, command.Angle, command.Length, SimulationManager.instance.m_currentBuildIndex++);

            ArrayHandler.StopApplying();
            BuildingHandler.IgnoreAll = false;
        }
    }
}

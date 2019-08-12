using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class BuildingRemoveHandler : CommandHandler<BuildingRemoveCommand>
    {
        public override void Handle(BuildingRemoveCommand command)
        {
            BuildingHandler.IgnoreAll = true;
            BuildingManager.instance.ReleaseBuilding(command.BuildingId);
            BuildingHandler.IgnoreAll = false;
        }
    }
}

using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class BuildingRelocateHandler : CommandHandler<BuildingRelocateCommand>
    {
        public override void Handle(BuildingRelocateCommand command)
        {
            BuildingHandler.IgnoreAll = true;
            BuildingManager.instance.RelocateBuilding(command.BuildingId, command.NewPosition, command.Angle);
            BuildingHandler.IgnoreAll = false;
        }
    }
}

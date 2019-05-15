
namespace CSM.Commands.Handler
{
    public class BuildingRemoveHandler : CommandHandler<BuildingRemoveCommand>
    {
        public override void Handle(BuildingRemoveCommand command)
        {
            Injections.BuildingHandler.lastRelease = command.BuildingId;
            BuildingManager.instance.ReleaseBuilding((ushort) command.BuildingId);
        }
    }
}

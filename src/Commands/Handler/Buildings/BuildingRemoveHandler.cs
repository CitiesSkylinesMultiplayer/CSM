using CSM.API.Commands;
using CSM.Commands.Data.Buildings;
using CSM.Helpers;

namespace CSM.Commands.Handler.Buildings
{
    public class BuildingRemoveHandler : CommandHandler<BuildingRemoveCommand>
    {
        protected override void Handle(BuildingRemoveCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            BuildingManager.instance.ReleaseBuilding(command.BuildingId);
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

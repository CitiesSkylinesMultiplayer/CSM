using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Buildings;

namespace CSM.BaseGame.Commands.Handler.Buildings
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

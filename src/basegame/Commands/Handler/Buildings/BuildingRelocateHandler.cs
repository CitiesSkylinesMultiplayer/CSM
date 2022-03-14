using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Buildings;

namespace CSM.BaseGame.Commands.Handler.Buildings
{
    public class BuildingRelocateHandler : CommandHandler<BuildingRelocateCommand>
    {
        protected override void Handle(BuildingRelocateCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            BuildingManager.instance.RelocateBuilding(command.BuildingId, command.NewPosition, command.Angle);
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

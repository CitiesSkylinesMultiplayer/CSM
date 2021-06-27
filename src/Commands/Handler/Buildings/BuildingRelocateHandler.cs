using CSM.API.Commands;
using CSM.Commands.Data.Buildings;
using CSM.Helpers;

namespace CSM.Commands.Handler.Buildings
{
    public class BuildingRelocateHandler : CommandHandler<BuildingRelocateCommand>
    {
        protected override void Handle(BuildingRelocateCommand command)
        {
            IgnoreHelper.StartIgnore();
            BuildingManager.instance.RelocateBuilding(command.BuildingId, command.NewPosition, command.Angle);
            IgnoreHelper.EndIgnore();
        }
    }
}

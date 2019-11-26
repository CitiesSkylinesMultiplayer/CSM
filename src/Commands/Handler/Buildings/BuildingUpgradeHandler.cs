using CSM.Commands.Data.Buildings;
using CSM.Helpers;
using CSM.Injections;

namespace CSM.Commands.Handler.Buildings
{
    public class BuildingUpgradeHandler : CommandHandler<BuildingUpgradeCommand>
    {
        protected override void Handle(BuildingUpgradeCommand command)
        {
            IgnoreHelper.StartIgnore();
            ArrayHandler.StartApplying(command.Array16Ids, command.Array32Ids);

            BuildingManager.instance.UpgradeBuilding(command.Building);

            ArrayHandler.StopApplying();
            IgnoreHelper.EndIgnore();
        }
    }
}

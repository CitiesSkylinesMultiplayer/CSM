using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Buildings;
using CSM.BaseGame.Injections;

namespace CSM.BaseGame.Commands.Handler.Buildings
{
    public class BuildingUpgradeHandler : CommandHandler<BuildingUpgradeCommand>
    {
        protected override void Handle(BuildingUpgradeCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            ArrayHandler.StartApplying(command.Array16Ids, command.Array32Ids);

            BuildingManager.instance.UpgradeBuilding(command.Building, false);

            ArrayHandler.StopApplying();
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

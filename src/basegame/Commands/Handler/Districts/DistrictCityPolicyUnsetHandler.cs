using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Districts;

namespace CSM.BaseGame.Commands.Handler.Districts
{
    public class DistrictCityPolicyUnsetHandler : CommandHandler<DistrictCityPolicyUnsetCommand>
    {
        protected override void Handle(DistrictCityPolicyUnsetCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            DistrictManager.instance.UnsetCityPolicy(command.Policy);
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

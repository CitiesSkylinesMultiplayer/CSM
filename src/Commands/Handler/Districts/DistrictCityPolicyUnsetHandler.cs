using CSM.API.Commands;
using CSM.Commands.Data.Districts;
using CSM.Helpers;

namespace CSM.Commands.Handler.Districts
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

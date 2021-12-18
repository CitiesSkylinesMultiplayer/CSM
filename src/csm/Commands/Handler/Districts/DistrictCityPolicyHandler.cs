using CSM.API.Commands;
using CSM.Commands.Data.Districts;
using CSM.Helpers;

namespace CSM.Commands.Handler.Districts
{
    public class DistrictCityPolicyHandler : CommandHandler<DistrictCityPolicyCommand>
    {
        protected override void Handle(DistrictCityPolicyCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            DistrictManager.instance.SetCityPolicy(command.Policy);
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

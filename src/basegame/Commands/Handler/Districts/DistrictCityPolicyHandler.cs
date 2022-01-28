using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Districts;

namespace CSM.BaseGame.Commands.Handler.Districts
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

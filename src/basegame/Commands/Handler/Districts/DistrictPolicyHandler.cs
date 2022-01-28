using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Districts;

namespace CSM.BaseGame.Commands.Handler.Districts
{
    public class DistrictPolicyHandler : CommandHandler<DistrictPolicyCommand>
    {
        protected override void Handle(DistrictPolicyCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            DistrictManager.instance.SetDistrictPolicy(command.Policy, command.DistrictId);
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

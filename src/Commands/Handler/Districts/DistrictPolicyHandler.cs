using CSM.API.Commands;
using CSM.Commands.Data.Districts;
using CSM.Helpers;

namespace CSM.Commands.Handler.Districts
{
    public class DistrictPolicyHandler : CommandHandler<DistrictPolicyCommand>
    {
        protected override void Handle(DistrictPolicyCommand command)
        {
            IgnoreHelper.StartIgnore();
            DistrictManager.instance.SetDistrictPolicy(command.Policy, command.DistrictId);
            IgnoreHelper.EndIgnore();
        }
    }
}

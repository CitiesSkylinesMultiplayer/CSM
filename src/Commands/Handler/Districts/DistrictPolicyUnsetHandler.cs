using CSM.API.Commands;
using CSM.Commands.Data.Districts;
using CSM.Helpers;

namespace CSM.Commands.Handler.Districts
{
    public class DistrictPolicyUnsetHandler : CommandHandler<DistrictPolicyUnsetCommand>
    {
        protected override void Handle(DistrictPolicyUnsetCommand command)
        {
            IgnoreHelper.StartIgnore();
            DistrictManager.instance.UnsetDistrictPolicy(command.Policy, command.DistrictId);
            IgnoreHelper.EndIgnore();
        }
    }
}

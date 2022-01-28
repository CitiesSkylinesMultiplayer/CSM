using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Districts;

namespace CSM.BaseGame.Commands.Handler.Districts
{
    public class DistrictPolicyUnsetHandler : CommandHandler<DistrictPolicyUnsetCommand>
    {
        protected override void Handle(DistrictPolicyUnsetCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            DistrictManager.instance.UnsetDistrictPolicy(command.Policy, command.DistrictId);
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

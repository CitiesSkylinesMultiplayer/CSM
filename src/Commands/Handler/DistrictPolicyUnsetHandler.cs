using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class DistrictPolicyUnsetHandler : CommandHandler<DistrictPolicyUnsetCommand>
    {
        public override void Handle(DistrictPolicyUnsetCommand command)
        {
            DistrictHandler.IgnoreAll = true;
            DistrictManager.instance.UnsetDistrictPolicy(command.Policy, command.DistrictId);
            DistrictHandler.IgnoreAll = false;
        }
    }
}

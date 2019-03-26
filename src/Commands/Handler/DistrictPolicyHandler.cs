using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class DistrictPolicyHandler : CommandHandler<DistrictPolicyCommand>
    {
        public override void Handle(DistrictPolicyCommand command)
        {
            DistrictHandler.IgnoreAll = true;
            DistrictManager.instance.SetDistrictPolicy(command.Policy, command.DistrictID);
            DistrictHandler.IgnoreAll = false;
        }
    }
}

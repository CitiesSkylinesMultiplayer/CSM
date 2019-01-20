using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class DistrictPolicyHandler : CommandHandler<DistrictPolicyCommand>
    {
        public override void Handle(DistrictPolicyCommand command)
        {
            DistrictHandler.IgnoreDistricts.Add(command.DistrictID);
            DistrictManager.instance.SetDistrictPolicy(command.Policy, command.DistrictID);
            DistrictHandler.IgnoreDistricts.Remove(command.DistrictID);
        }
    }
}

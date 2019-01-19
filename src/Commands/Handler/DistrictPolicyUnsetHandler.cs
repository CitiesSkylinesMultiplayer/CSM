using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class DistrictPolicyUnsetHandler : CommandHandler<DistrictPolicyUnsetCommand>
    {
        public override void Handle(DistrictPolicyUnsetCommand command)
        {
            DistrictHandler.IgnoreDistricts.Add(command.DistrictID);
            DistrictManager.instance.UnsetDistrictPolicy(command.Policy, command.DistrictID);
            DistrictHandler.IgnoreDistricts.Remove(command.DistrictID);
        }

    }
}

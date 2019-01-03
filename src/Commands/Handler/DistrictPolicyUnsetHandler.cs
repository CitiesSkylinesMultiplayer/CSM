using CSM.Injections;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class DistrictPolicyUnsetHandler : CommandHandler<DistrictPolicyUnsetCommand>
    {

        public override byte ID => CommandIds.DistrictPolicyUnsetCommand;

        public override void HandleOnClient(DistrictPolicyUnsetCommand command) => Handle(command);

        public override void HandleOnServer(DistrictPolicyUnsetCommand command, Player player) => Handle(command);

        private void Handle(DistrictPolicyUnsetCommand command)
        {
            DistrictHandler.IgnoreDistricts.Add(command.DistrictID);
            DistrictManager.instance.UnsetDistrictPolicy(command.Policy, command.DistrictID);
            DistrictHandler.IgnoreDistricts.Remove(command.DistrictID);
        }

    }
}

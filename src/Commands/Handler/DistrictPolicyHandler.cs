using CSM.Injections;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class DistrictPolicyHandler : CommandHandler<DistrictPolicyCommand>
    {
        public override byte ID => CommandIds.DistrictPolicyCommand;

        public override void HandleOnClient(DistrictPolicyCommand command) => Handle(command);

        public override void HandleOnServer(DistrictPolicyCommand command, Player player) => Handle(command);

        private void Handle(DistrictPolicyCommand command)
        {
            DistrictHandler.IgnoreDistricts.Add(command.DistrictID);
            DistrictManager.instance.SetDistrictPolicy(command.Policy, command.DistrictID);
            DistrictHandler.IgnoreDistricts.Remove(command.DistrictID);
        }
    }
}

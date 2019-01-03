using CSM.Injections;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class DistrictCityPolicyHandler : CommandHandler<DistrictCityPolicyCommand>
    {
        public override byte ID => CommandIds.DistrictCityPolicyCommand;

        public override void HandleOnClient(DistrictCityPolicyCommand command) => Handle(command);

        public override void HandleOnServer(DistrictCityPolicyCommand command, Player player) => Handle(command);

        private void Handle(DistrictCityPolicyCommand command)
        {
            DistrictHandler.ignoreCityPolicy = true;
            DistrictManager.instance.SetCityPolicy(command.Policy);
            DistrictHandler.ignoreCityPolicy = false;
        }
    }
}

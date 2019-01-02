using CSM.Injections;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class CityPolicyHandler : CommandHandler<CityPolicyCommand>
    {
        public override byte ID => CommandIds.CityPolicyCommand;

        public override void HandleOnClient(CityPolicyCommand command) => Handle(command);

        public override void HandleOnServer(CityPolicyCommand command, Player player) => Handle(command);

        private void Handle(CityPolicyCommand command)
        {
            DistrictHandler.ignoreCityPolicy = true;
            DistrictManager.instance.SetCityPolicy(command.Policy);
            DistrictHandler.ignoreCityPolicy = false;
        }
    }
}

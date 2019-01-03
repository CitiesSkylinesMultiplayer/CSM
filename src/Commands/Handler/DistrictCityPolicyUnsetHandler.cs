using CSM.Injections;
using CSM.Networking;


namespace CSM.Commands.Handler
{
    class DistrictCityPolicyUnsetHandler : CommandHandler<DistrictCityPolicyUnsetCommand>
    {
        public override byte ID => CommandIds.DistrictCityPolicyUnsetCommand;

        public override void HandleOnClient(DistrictCityPolicyUnsetCommand command) => Handle(command);

        public override void HandleOnServer(DistrictCityPolicyUnsetCommand command, Player player) => Handle(command);

        private void Handle(DistrictCityPolicyUnsetCommand command)
        {
            DistrictHandler.ignoreCityPolicy = true;
            DistrictManager.instance.UnsetCityPolicy(command.Policy);
            DistrictHandler.ignoreCityPolicy = false;
        }

    }
}

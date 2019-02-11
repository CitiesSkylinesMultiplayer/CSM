using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class DistrictCityPolicyUnsetHandler : CommandHandler<DistrictCityPolicyUnsetCommand>
    {
        public override void Handle(DistrictCityPolicyUnsetCommand command)
        {
            DistrictHandler.IgnoreAll = true;
            DistrictManager.instance.UnsetCityPolicy(command.Policy);
            DistrictHandler.IgnoreAll = false;
        }
    }
}

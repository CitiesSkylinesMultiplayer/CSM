using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class DistrictCityPolicyHandler : CommandHandler<DistrictCityPolicyCommand>
    {
        public override void Handle(DistrictCityPolicyCommand command)
        {
            DistrictHandler.IgnoreAll = true;
            DistrictManager.instance.SetCityPolicy(command.Policy);
            DistrictHandler.IgnoreAll = false;
        }
    }
}

using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class DistrictCreateHandler : CommandHandler<DistrictCreateCommand>
    {
        public override void Handle(DistrictCreateCommand command)
        {
            DistrictHandler.IgnoreDistricts.Add(command.DistrictID);
            DistrictManager.instance.CreateDistrict(out byte district);
            DistrictHandler.IgnoreDistricts.Remove(command.DistrictID);
        }
    }
}

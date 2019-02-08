using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class DistrictCreateHandler : CommandHandler<DistrictCreateCommand>
    {
        public override void Handle(DistrictCreateCommand command)
        {
            DistrictHandler.IgnoreAll = true;
            DistrictManager.instance.CreateDistrict(out byte district);
            DistrictHandler.IgnoreAll = false;
        }
    }
}

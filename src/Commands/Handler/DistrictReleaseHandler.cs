using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class DistrictReleaseHandler : CommandHandler<DistrictReleaseCommand>
    {
        public override void Handle(DistrictReleaseCommand command)
        {
            DistrictHandler.IgnoreAll = true;
            DistrictManager.instance.ReleaseDistrict(command.DistrictID);
            DistrictHandler.IgnoreAll = false;
        }
    }
}

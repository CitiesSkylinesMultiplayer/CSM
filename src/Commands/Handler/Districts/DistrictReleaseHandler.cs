using CSM.Commands.Data.Districts;
using CSM.Helpers;

namespace CSM.Commands.Handler.Districts
{
    public class DistrictReleaseHandler : CommandHandler<DistrictReleaseCommand>
    {
        protected override void Handle(DistrictReleaseCommand command)
        {
            IgnoreHelper.StartIgnore();
            DistrictManager.instance.ReleaseDistrict(command.DistrictId);
            IgnoreHelper.EndIgnore();
        }
    }
}

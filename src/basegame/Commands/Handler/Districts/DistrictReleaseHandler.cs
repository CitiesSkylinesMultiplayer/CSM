using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Districts;

namespace CSM.BaseGame.Commands.Handler.Districts
{
    public class DistrictReleaseHandler : CommandHandler<DistrictReleaseCommand>
    {
        protected override void Handle(DistrictReleaseCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            DistrictManager.instance.ReleaseDistrict(command.DistrictId);
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

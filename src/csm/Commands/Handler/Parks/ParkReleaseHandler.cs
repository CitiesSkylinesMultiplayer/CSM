using CSM.API.Commands;
using CSM.Commands.Data.Parks;
using CSM.Helpers;

namespace CSM.Commands.Handler.Parks
{
    public class ParkReleaseHandler : CommandHandler<ParkReleaseCommand>
    {
        protected override void Handle(ParkReleaseCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            DistrictManager.instance.ReleasePark(command.ParkId);
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

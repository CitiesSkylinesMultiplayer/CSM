using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Parks;

namespace CSM.BaseGame.Commands.Handler.Parks
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

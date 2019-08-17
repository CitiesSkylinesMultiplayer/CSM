using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class ParkReleaseHandler : CommandHandler<ParkReleaseCommand>
    {
        public override void Handle(ParkReleaseCommand command)
        {
            DistrictHandler.IgnoreAll = true;
            DistrictManager.instance.ReleasePark(command.ParkId);
            DistrictHandler.IgnoreAll = false;
        }
    }
}

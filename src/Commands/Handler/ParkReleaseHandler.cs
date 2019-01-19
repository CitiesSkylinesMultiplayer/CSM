using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class ParkReleaseHandler : CommandHandler<ParkReleaseCommand>
    {
        public override void Handle(ParkReleaseCommand command)
        {
            DistrictHandler.IgnoreParks.Add(command.ParkID);
            DistrictManager.instance.ReleasePark(command.ParkID);
            DistrictHandler.IgnoreParks.Remove(command.ParkID);
        }

    }
}

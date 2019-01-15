using CSM.Injections;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class ParkReleaseHandler : CommandHandler<ParkReleaseCommand>
    {
        public override byte ID => CommandIds.ParkReleaseCommand;

        public override void HandleOnClient(ParkReleaseCommand command) => Handle(command);

        public override void HandleOnServer(ParkReleaseCommand command, Player player) => Handle(command);

        private void Handle(ParkReleaseCommand command)
        {
            DistrictHandler.IgnoreParks.Add(command.ParkID);
            DistrictManager.instance.ReleasePark(command.ParkID);
            DistrictHandler.IgnoreParks.Remove(command.ParkID);
        }

    }
}

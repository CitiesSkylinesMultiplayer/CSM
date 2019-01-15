using CSM.Injections;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class ParkCreateHandler : CommandHandler<ParkCreateCommand>
    {
        public override byte ID => CommandIds.ParkCreateCommand;

        public override void HandleOnClient(ParkCreateCommand command) => Handle(command);

        public override void HandleOnServer(ParkCreateCommand command, Player player) => Handle(command);

        private void Handle(ParkCreateCommand command)
        {
            DistrictHandler.IgnoreParks.Add(command.ParkID);
            DistrictManager.instance.CreatePark(out byte Park, command.ParkType, command.ParkLevel);
            DistrictHandler.IgnoreParks.Remove(command.ParkID);
        }
    }
}

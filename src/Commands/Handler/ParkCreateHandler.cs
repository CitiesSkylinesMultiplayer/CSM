using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class ParkCreateHandler : CommandHandler<ParkCreateCommand>
    {
        public override void Handle(ParkCreateCommand command)
        {
            DistrictHandler.IgnoreParks.Add(command.ParkID);
            DistrictManager.instance.CreatePark(out byte Park, command.ParkType, command.ParkLevel);
            DistrictHandler.IgnoreParks.Remove(command.ParkID);
        }
    }
}

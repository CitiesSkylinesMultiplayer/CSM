using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class ParkCreateHandler : CommandHandler<ParkCreateCommand>
    {
        public override void Handle(ParkCreateCommand command)
        {
            DistrictHandler.IgnoreAll = true;
            DistrictManager.instance.CreatePark(out byte Park, command.ParkType, command.ParkLevel);
            DistrictHandler.IgnoreAll = false;
        }
    }
}

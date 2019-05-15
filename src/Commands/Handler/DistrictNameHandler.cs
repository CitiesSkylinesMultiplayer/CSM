using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class DistrictNameHandler : CommandHandler<DistrictNameCommand>
    {
        public override void Handle(DistrictNameCommand command)
        {
            DistrictHandler.IgnoreAll = true;
            while (DistrictManager.instance.SetDistrictName(command.DistrictID, command.Name).MoveNext()) { break; }
            DistrictHandler.IgnoreAll = false;
        }
    }
}

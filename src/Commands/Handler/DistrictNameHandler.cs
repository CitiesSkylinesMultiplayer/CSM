using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class DistrictNameHandler : CommandHandler<DistrictNameCommand>
    {
        public override void Handle(DistrictNameCommand command)
        {
            DistrictHandler.IgnoreAll = true;
            DistrictManager.instance.SetDistrictName(command.DistrictID, command.Name);
            DistrictHandler.IgnoreAll = false;
        }
    }
}

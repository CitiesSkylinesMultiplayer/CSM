using CSM.GS.Commands.Data.ApiServer;

namespace CSM.GS.Commands.Handler.ApiServer
{
    public class ServerRegistrationHandler : ApiCommandHandler<ServerRegistrationCommand>
    {
        protected override void Handle(ServerRegistrationCommand command)
        {
            // Do nothing, this is a packet for the global server
        }
    }
}

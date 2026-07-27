using CSM.GS.Commands.Data.ApiServer;

namespace CSM.GS.Commands.Handler.ApiServer
{
    public class ServerListRequestHandler : ApiCommandHandler<ServerListRequestCommand>
    {
        protected override void Handle(ServerListRequestCommand command)
        {
            // Do nothing, this is a packet for the global server
        }
    }
}

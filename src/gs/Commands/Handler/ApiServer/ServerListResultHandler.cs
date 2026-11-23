using CSM.GS.Commands.Data.ApiServer;

namespace CSM.GS.Commands.Handler.ApiServer
{
    public class ServerListResultHandler : ApiCommandHandler<ServerListResultCommand>
    {
        protected override void Handle(ServerListResultCommand command)
        {
            // Do nothing, this is a packet for the browsing client
        }
    }
}

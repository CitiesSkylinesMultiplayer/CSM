using CSM.GS.Commands.Data.ApiServer;

namespace CSM.GS.Commands.Handler.ApiServer
{
    public class PortCheckRequestHandler : ApiCommandHandler<PortCheckRequestCommand>
    {
        protected override void Handle(PortCheckRequestCommand command)
        {
            // Do nothing, this is a packet for the global server
        }
    }
}

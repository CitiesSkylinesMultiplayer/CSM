using CSM.GS.Commands.Data.ApiServer;

namespace CSM.GS.Commands.Handler.ApiServer
{
    public class PortCheckResultHandler : ApiCommandHandler<PortCheckResultCommand>
    {
        protected override void Handle(PortCheckResultCommand command)
        {
            // Do nothing, this is a packet for the CSM server
        }
    }
}

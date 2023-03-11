using System.Net;
using CSM.GS.Commands.Data.ApiServer;

namespace CSM.GS.Commands.Handler.ApiServer
{
    public class ServerRegistrationHandler : ApiCommandHandler<ServerRegistrationCommand>
    {
        protected override void Handle(ServerRegistrationCommand command)
        {
            worker.RegisterServer(sender, new IPEndPoint(IPAddress.Parse(command.LocalIp), command.LocalPort), command.Token);
        }
    }
}

using System.Net;
using System.Threading;
using CSM.GS.Commands.Data.ApiServer;

namespace CSM.GS.Commands.Handler.ApiServer
{
    public class PortCheckRequestHandler : ApiCommandHandler<PortCheckRequestCommand>
    {
        protected override void Handle(PortCheckRequestCommand command)
        {
            IPEndPoint endPoint = new(sender.Address, command.Port);
            new Thread(() =>
            {
                bool result = new Client(worker).Connect(endPoint);
                worker.QueueAction(() =>
                {
                    worker.SendToServer(sender, new PortCheckResultCommand
                    {
                        State = result ? PortCheckResult.Reachable : PortCheckResult.Unreachable,
                        Message = null
                    });
                });
            }).Start();
        }
    }
}

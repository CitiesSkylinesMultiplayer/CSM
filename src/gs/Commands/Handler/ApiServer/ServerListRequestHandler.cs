using System.Linq;
using CSM.GS.Commands.Data.ApiServer;

namespace CSM.GS.Commands.Handler.ApiServer
{
    public class ServerListRequestHandler : ApiCommandHandler<ServerListRequestCommand>
    {
        protected override void Handle(ServerListRequestCommand command)
        {
            PublicServerEntry[] servers = worker.PublicServers.Select(server => new PublicServerEntry
            {
                Name = server.ServerName,
                CurrentPlayers = server.CurrentPlayers,
                MaxPlayers = server.MaxPlayers,
                HasPassword = server.HasPassword,
                ServerToken = server.Token
            }).ToArray();

            worker.SendToServer(sender, new ServerListResultCommand { Servers = servers });
        }
    }
}

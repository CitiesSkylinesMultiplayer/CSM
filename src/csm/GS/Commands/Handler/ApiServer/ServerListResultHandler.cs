using CSM.GS.Commands.Data.ApiServer;
using CSM.Panels;

namespace CSM.GS.Commands.Handler.ApiServer
{
    public class ServerListResultHandler : ApiCommandHandler<ServerListResultCommand>
    {
        protected override void Handle(ServerListResultCommand command)
        {
            PanelManager.GetPanel<BrowseServersPanel>()?.OnServerListReceived(command.Servers ?? new PublicServerEntry[0]);
        }
    }
}

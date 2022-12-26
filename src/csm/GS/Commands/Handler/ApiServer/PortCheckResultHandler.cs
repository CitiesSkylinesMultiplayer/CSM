using CSM.API;
using CSM.GS.Commands.Data.ApiServer;
using CSM.Networking;
using CSM.Panels;

namespace CSM.GS.Commands.Handler.ApiServer
{
    public class PortCheckResultHandler : ApiCommandHandler<PortCheckResultCommand>
    {
        protected override void Handle(PortCheckResultCommand command)
        {
            string vpnIp = IpAddress.GetVPNIpAddress();
            bool automaticSuccess = MultiplayerManager.Instance.CurrentServer.AutomaticSuccess;
            string message;
            bool portOpen = false;
            switch (command.State)
            {
                case PortCheckResult.Unreachable:
                    if (vpnIp != null)
                    {
                        message =
                            "Server is not reachable from the internet. Players can connect over Hamachi using the IP address " +
                            vpnIp;
                        portOpen = true; // This is an OK state
                    }
                    else if (automaticSuccess)
                    {
                        message =
                                "It was tried to forward the port automatically, but the server is not reachable from the internet. Manual port forwarding is required. See the \"Manage Server\" menu for more info.";
                    }
                    else
                    {
                        message =
                            "Port could not be forwarded automatically and server is not reachable from the internet. Manual port forwarding is required. See the \"Manage Server\" menu for more info.";
                    }
                    break;
                case PortCheckResult.Reachable:
                    portOpen = true;
                    if (automaticSuccess)
                    {
                        message =
                            "Port was forwarded automatically and server is reachable from the internet!";
                    }
                    else
                    {
                        message = "Server is reachable from the internet!";
                    }

                    if (vpnIp != null)
                    {
                        message += " Players can also connect over Hamachi, although you don't need it.";
                    }
                    break;
                case PortCheckResult.Error:
                    if (vpnIp != null)
                    {
                        message =
                            "Error while checking server port from the internet. Players should still be able to connect over the Hamachi IP address " +
                            vpnIp;
                    }
                    else if (automaticSuccess)
                    {
                        message = "Port was forwarded automatically, but couldn't be checked due to error: " +
                                  command.Message;
                    }
                    else
                    {
                        message = "Port could not be forwarded automatically, and couldn't be checked due to error: " +
                                  command.Message;
                    }
                    break;
                default:
                    message = "Unknown response while checking server port from the internet.";
                    break;
            }

            if (!portOpen)
            {
                Log.Warn(message);
            }
            Chat.Instance.PrintGameMessage(portOpen ? Chat.MessageType.Normal : Chat.MessageType.Warning, message);

            PanelManager.GetPanel<ManageGamePanel>()?.SetPortState(command);
        }
    }
}

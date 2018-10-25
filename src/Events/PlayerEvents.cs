using CSM.Networking;

namespace CSM.Events
{
    public delegate void PlayerConnectEventHandler(Server server, PlayerEventArgs args);

    public delegate void PlayerDisconnectEventHandler(Server server, PlayerEventArgs args);
}
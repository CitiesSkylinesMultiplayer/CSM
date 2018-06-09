using System;
using LiteNetLib;

namespace CitiesSkylinesMultiplayer.Testing
{
    class Program
    {
        #region Private Variables
        private EventBasedNetListener _listener;
        private NetManager _netClient;
        #endregion

        static void Main(string[] args)
        {
            new Program().Start(args);
        }

        private void Start(string[] args)
        {
            // Setup the listener and bind events
            _listener = new EventBasedNetListener();
            _listener.NetworkReceiveEvent += _listener_NetworkReceiveEvent;
            _listener.NetworkErrorEvent += ListenerOnNetworkErrorEvent;

            // Create the client
            _netClient = new NetManager(_listener, "TangoAlpha");
            var result = _netClient.Start();

            // Connect the client
            var connection = _netClient.Connect(IpBox.Text, int.Parse(PortBox.Text));

        }

        private void ListenerOnNetworkErrorEvent(NetEndPoint endpoint, int socketerrorcode)
        {
            // On Error, show a message box
            Console.WriteLine($"[{endpoint.Host}:{endpoint.Port}] Socket Error Code: {socketerrorcode}");
        }

        private void _listener_NetworkReceiveEvent(NetPeer peer, LiteNetLib.Utils.NetDataReader reader)
        {
            Console.WriteLine($"[{peer.EndPoint.Host}:{peer.EndPoint.Port}] {reader.GetString()}");
        }
    }
}

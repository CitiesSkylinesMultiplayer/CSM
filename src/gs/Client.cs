using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using LiteNetLib;

namespace CSM.GS
{
    public class Client
    {
        private readonly NetManager _netClient;
        private ClientStatus _status = ClientStatus.Disconnected;
        private WorkerService _worker;

        public Client(WorkerService worker)
        {
            _worker = worker;
            // Set up network items
            EventBasedNetListener listener = new();
            _netClient = new NetManager(listener)
            {
                NatPunchEnabled = true,
                UnconnectedMessagesEnabled = true
            };

            // Listen to events
            listener.PeerConnectedEvent += ListenerOnPeerConnectedEvent;
            listener.PeerDisconnectedEvent += ListenerOnPeerDisconnectedEvent;
        }

        public bool Connect(IPEndPoint target)
        {
            _status = ClientStatus.PreConnecting;

            bool result = _netClient.Start();
            if (!result)
            {
                return false;
            }

            bool success = SetupHolePunching(target);
            _netClient.Stop();

            return success;
        }

        private bool SetupHolePunching(IPEndPoint target)
        {
            EventBasedNatPunchListener natPunchListener = new();
            Stopwatch timeoutWatch = new();

            // Callback on for each possible IP address to connect to the server.
            // Can potentially be called multiple times (local and public IP address).
            natPunchListener.NatIntroductionSuccess += (point, _, _) =>
            {
                timeoutWatch.Stop();

                if (_status == ClientStatus.PreConnecting)
                {
                    bool success = DoConnect(point);
                    if (!success)
                    {
                        _status = ClientStatus.PreConnecting;
                    }
                }

                timeoutWatch.Start();
            };

            // Register listener and send request to global server
            _netClient.NatPunchModule.Init(natPunchListener);
            _netClient.NatPunchModule.SendNatIntroduceRequest(new IPEndPoint(_worker.RemoteServerAddress, _worker.ServerPort), target.Address.ToString());

            timeoutWatch.Start();
            // Wait for NatPunchModule responses.
            // 5 seconds include only the time waiting for nat punch management.
            // Connection attempts have their own timeout in the DoConnect method
            // The waitWatch is paused during an connection attempt.
            while (_status == ClientStatus.PreConnecting && timeoutWatch.Elapsed < TimeSpan.FromSeconds(5))
            {
                _netClient.PollEvents();
                _netClient.NatPunchModule.PollEvents();
                Thread.Sleep(50);
            }

            if (_status == ClientStatus.PreConnecting) // If timeout, try exact given address
            {
                bool success = DoConnect(target);
                if (!success)
                {
                    _status = ClientStatus.Disconnected;
                    return false;
                }

                return true;
            }

            return _status == ClientStatus.Connected;
        }

        private bool DoConnect(IPEndPoint point)
        {
            try
            {
                _netClient.Connect(point, "CSM");
            }
            catch (Exception)
            {
                return false;
            }

            // Start processing networking
            _status = ClientStatus.Connecting;

            Stopwatch waitWatch = new();
            waitWatch.Start();

            // Try connect for 10 seconds
            while (waitWatch.Elapsed < TimeSpan.FromSeconds(10))
            {
                _netClient.PollEvents();
                // If we connect, exit the loop and return true
                if (_status == ClientStatus.Connected)
                {
                    return true;
                }

                // The client cannot connect for some reason
                if (_status == ClientStatus.Disconnected)
                {
                    return false;
                }
                // Wait 250ms
                Thread.Sleep(100);
            }
            return false;
        }

        private void ListenerOnPeerConnectedEvent(NetPeer peer)
        {
            _status = ClientStatus.Connected;
        }

        private void ListenerOnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            _status = ClientStatus.Disconnected;
        }
    }
}

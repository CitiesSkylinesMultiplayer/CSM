using ColossalFramework;
using CSM.Commands;
using CSM.Helpers;
using CSM.Networking.Config;
using CSM.Networking.Status;
using LiteNetLib;
using LiteNetLib.Utils;
using Open.Nat;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace CSM.Networking
{
    /// <summary>
    ///     Server
    /// </summary>
    public class Server
    {
        // The server
        public LiteNetLib.NetManager NetServer { get; }

        // Run a background processing thread
        private Thread _serverProcessingThread;

        // Config options for server
        private ServerConfig _serverConfig;

        // Timer for handling ping
        private System.Timers.Timer _pingTimer;

        /// <summary>
        ///     The current status of the server
        /// </summary>
        public ServerStatus Status { get; private set; }

        public Server()
        {
            // Set up network items
            var listener = new EventBasedNetListener();
            NetServer = new LiteNetLib.NetManager(listener, "Tango");

            // Listen to events
            listener.NetworkReceiveEvent += ListenerOnNetworkReceiveEvent;
            listener.NetworkErrorEvent += ListenerOnNetworkErrorEvent;

            // Setup timer
            _pingTimer = new System.Timers.Timer();
            _pingTimer.Elapsed += OnPing;
            _pingTimer.Interval = 100;
            _pingTimer.Start();
        }

        /// <summary>
        ///     Starts the server with the specified config options
        /// </summary>
        /// <param name="serverConfig">Server config information</param>
        /// <returns>If the server has started.</returns>
        public bool StartServer(ServerConfig serverConfig)
        {
            // If the server is already running, we will stop and start it again
            if (Status == ServerStatus.Running)
                StopServer();

            // Set the config
            _serverConfig = serverConfig;

            // Let the user know that we are trying to start the server
            CSM.Log($"Attempting to start server on port {_serverConfig.Port}...");

            // Attempt to start the server
            NetServer.DiscoveryEnabled = true;
            var result = NetServer.Start(_serverConfig.Port);

            // If the server has not started, tell the user and return false.
            if (!result)
            {
                CSM.Log("The server failed to start.");
                StopServer(); // Make sure the server is fully stopped
                return false;
            }

            try
            {
                // This async stuff is nasty, but we have to target .net 3.5 (unless cities skylines upgrades to something higher).
                var nat = new NatDiscoverer();
                var cts = new CancellationTokenSource();
                cts.CancelAfter(5000);

                // No idea if this even works
                nat.DiscoverDeviceAsync(PortMapper.Upnp, cts).ContinueWith(task => task.Result.CreatePortMapAsync(new Mapping(Protocol.Udp, _serverConfig.Port,
                    _serverConfig.Port, "Cities Skylines Multiplayer (UDP)"))).Wait();
            }
            catch (Exception e)
            {
                CSM.Log("Failed to automatically open port. Manual Port Forwarding is required. " + e.Message);
            }

            // Update the status
            Status = ServerStatus.Running;

            // Set up processing thread
            _serverProcessingThread = new Thread(ProcessEvents);
            _serverProcessingThread.Start();

            // Update the console to let the user know the server is running
            CSM.Log("The server has started.");
            return true;
        }

        /// <summary>
        ///     Stops the server
        /// </summary>
        public void StopServer()
        {
            // Update status and stop the server
            Status = ServerStatus.Stopped;
            NetServer.Stop();

            CSM.Log("Stopped server");
        }

        /// <summary>
        ///     Send a message to all connected clients.
        /// </summary>
        /// <param name="messageId">Message type/id</param>
        /// <param name="message">The actual message</param>
        public void SendToClients(byte messageId, CommandBase message)
        {
            if (Status != ServerStatus.Running)
                return;

            NetServer.SendToAll(ArrayHelpers.PrependByte(messageId, message.Serialize()), SendOptions.ReliableOrdered);
        }

        /// <summary>
        ///     Send a message to a specific client
        /// </summary>
        public void SendToClient(NetPeer peer, byte messageId, CommandBase message)
        {
            if (Status != ServerStatus.Running)
                return;

            peer.Send(ArrayHelpers.PrependByte(messageId, message.Serialize()), SendOptions.ReliableOrdered);
        }

        /// <summary>
        ///     Runs in the background of the game (another thread), polls for new updates
        ///     from the clients.
        /// </summary>
        private void ProcessEvents()
        {
            while (Status == ServerStatus.Running)
            {
                // Poll for new events
                NetServer.PollEvents();

                // Wait
                Thread.Sleep(15);
            }
        }

        /// <summary>
        ///     Ping all connected clients
        /// </summary>
        private void OnPing(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Server not running, don't send ping
            if (Status == ServerStatus.Stopped)
                return;

            // Loop though all connected peers
            foreach (var netPeer in NetServer.GetPeers())
            {
                // Send a ping
                netPeer.Send(ArrayHelpers.PrependByte(CommandBase.PingCommandId, new PingCommand().Serialize()), SendOptions.ReliableOrdered);
            }
        }

        /// <summary>
        ///     When we get a message from a client, we handle the message here
        ///     and perform any necessary tasks.
        /// </summary>
        private void ListenerOnNetworkReceiveEvent(NetPeer peer, NetDataReader reader)
        {
            try
            {
                // The message type is the first byte, (255 message types)
                var messageType = reader.Data[0];

                // Skip the first byte
                var message = reader.Data.Skip(1).ToArray();

                // Switch between all the messages
                switch (messageType)
                {
                    case CommandBase.ConnectionRequestCommandId:

                        var connectionResult = Commands.ConnectionRequestCommand.Deserialize(message);

                        CSM.Log($"Connection request from {peer.EndPoint.Host}:{peer.EndPoint.Port}. Version: {connectionResult.GameVersion}, ModCount: {connectionResult.ModCount}, ModVersion: {connectionResult.ModVersion}");

                        // TODO, check these values, but for now, just accept the request.
                        SendToClient(peer, CommandBase.ConnectionResultCommandId, new ConnectionResultCommand { Success = true });
                        break;

                    case CommandBase.PauseCommandID:
                        var pause = PauseCommand.Deserialize(message);
                        SimulationManager.instance.SimulationPaused = pause.SimulationPaused;
                        break;

                    case CommandBase.SpeedCommandID:
                        var speed = SpeedCommand.Deserialize(message);
                        SimulationManager.instance.SelectedSimulationSpeed = speed.SelectedSimulationSpeed;
                        break;

                    case CommandBase.MoneyCommandID:
                        var internalMoney = MoneyCommand.Deserialize(message);
                        typeof(EconomyManager).GetField("m_cashAmount", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, internalMoney.InternalMoneyAmount);
                        typeof(EconomyManager).GetField("m_lastCashAmount", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, internalMoney.InternalMoneyAmount);
                        break;
                }
            }
            catch (Exception ex)
            {
                CSM.Log($"Received an error from {peer.EndPoint.Host}:{peer.EndPoint.Port}. Message: {ex.Message}");
            }
        }

        /// <summary>
        ///     Called whenever an error happens, we
        ///     log this to the console for now.
        /// </summary>
        private void ListenerOnNetworkErrorEvent(NetEndPoint endpoint, int socketerrorcode)
        {
            CSM.Log($"Received an error from {endpoint.Host}:{endpoint.Port}. Code: {socketerrorcode}");
        }
    }
}
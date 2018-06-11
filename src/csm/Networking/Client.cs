using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using CSM.Commands;
using CSM.Helpers;
using CSM.Networking.Config;
using CSM.Networking.Status;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using PingCommand = CSM.Commands.PingCommand;

namespace CSM.Networking
{
    /// <summary>
    ///     Client
    /// </summary>
    public class Client
    {
        // The client
        public LiteNetLib.NetManager NetClient { get; }

        // Run a background processing thread
        private Thread _clientProcessingThread;

        // Config options for server
        private ClientConfig _clientConfig;

        // The last time we recieved a message from the server
        private DateTime _lastServerPing;

        // Timer to make sure the client is already connected to the server
        private System.Timers.Timer _pingTimer;

        /// <summary>
        ///     The current status of the client
        /// </summary>
        public ClientStatus Status { get; private set; }

        /// <summary>
        ///     If the status is disconnected, this will contain
        ///     the reason why.
        /// </summary>
        public string ConnectionMessage { get; private set; }

        public Client()
        {
            // Set up network items
            var listener = new EventBasedNetListener();
            NetClient = new LiteNetLib.NetManager(listener, "Tango");

            // Listen to events
            listener.NetworkReceiveEvent += ListenerOnNetworkReceiveEvent;
            listener.NetworkErrorEvent += ListenerOnNetworkErrorEvent;
            listener.PeerConnectedEvent += ListenerOnPeerConnectedEvent;

            // Setup timer
            _pingTimer = new System.Timers.Timer();
            _pingTimer.Elapsed += OnPing;
            _pingTimer.Interval = 1000;
        }

        /// <summary>
        ///     Attempt to connect to a server
        /// </summary>
        /// <param name="clientConfig">Client config params</param>
        /// <returns>True if the client is connected to the server, false if not</returns>
        public bool Connect(ClientConfig clientConfig)
        {
            // if we are currently trying to connect, cancel
            // and try again.
            if (Status == ClientStatus.Connecting)
                Disconnect();

            // The client is already connected so we need to 
            // disconnect.
            if (Status == ClientStatus.Connected)
                Disconnect();

            // Set the config
            _clientConfig = clientConfig;

            // Let the user know that we are trying to connect to a server
            CSM.Log($"Attempting to connect to server at {_clientConfig.HostAddress}:{_clientConfig.Port}...");

            // Start the client, if client setup fails, return out and 
            // tell the user
            var result = NetClient.Start();
            if (!result)
            {
                CSM.Log("The client failed to start.");
                ConnectionMessage = "The client failed to start.";
                Disconnect(); // make sure we are fully disconnected
                return false;
            }

            // Try connect to server, update the status to say that
            // we are trying to connect.
            NetClient.Connect(_clientConfig.HostAddress, _clientConfig.Port);

            // Start processing networking
            Status = ClientStatus.Connecting;

            // Setup processing thread
            _clientProcessingThread = new Thread(ProcessEvents);
            _clientProcessingThread.Start();
            _pingTimer.Start();

            // We need to wait in a loop for 30 seconds (waiting 500ms each time)
            // while we wait for a successful connection (Status = Connected) or a 
            // failed connection (Status = Disconnected).
            var waitWatch = new Stopwatch();
            waitWatch.Start();

            while (waitWatch.Elapsed < TimeSpan.FromSeconds(30))
            {
                // If we connect, exit the loop and return true
                if (Status == ClientStatus.Connected)
                    return true;

                // The client cannot connect for some reason, the ConnectionMessage
                // variable will contain why.
                if (Status == ClientStatus.Disconnected)
                {
                    Disconnect(); // make sure we are fully disconnected
                    return false;
                }

                // Wait 500ms
                Thread.Sleep(500);
            }

            // We have timed out
            ConnectionMessage = "Could not connect to server, timed out.";
            CSM.Log("Could not connect to server, timed out.");

            // Did not connect
            Disconnect(); // make sure we are fully disconnected
            return false;
        }

        /// <summary>
        ///     Attempt to disconnect from the server
        /// </summary>
        /// <returns></returns>
        public void Disconnect() 
        {
            // Update status and stop client
            Status = ClientStatus.Disconnected;
            NetClient.Stop();

            _pingTimer.Stop();

            CSM.Log("Disconnected from server.");
        }

        public void SendToServer(byte messageId, CommandBase message)
        {
            if (Status == ClientStatus.Disconnected)
                return;

            NetClient.GetFirstPeer().Send(ArrayHelpers.PrependByte(messageId, message.Serialize()), SendOptions.ReliableOrdered);
        }

        /// <summary>
        ///     Runs in the background of the game (another thread), polls for new updates
        ///     from the server.
        /// </summary>
        private void ProcessEvents()
        {
            // Run this loop while we are connected or if we are still trying to connect.
            while (Status == ClientStatus.Connected || Status == ClientStatus.Connecting)
            {
                // Poll for new events
                NetClient.PollEvents();

                // Wait
                Thread.Sleep(15);     
            }
        }

        /// <summary>
        ///     Check if we are still conencted to the server
        /// </summary>
        private void OnPing(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Client not connected, don't worry about this code
            if (Status == ClientStatus.Disconnected)
                return;

            // If we have not heard from the server in 5 seconds, it's probably gone
            // for now, we just disconnect. In the future we should try reconnecting and
            // displaying a UI.
            if (DateTime.UtcNow - _lastServerPing >= TimeSpan.FromSeconds(5))
            {
                CSM.Log("Client lost connection with the server (time out, last ping > 5 seconds). Disconnecting...");
                Disconnect();
            }
        }

        /// <summary>
        ///     When we get a message from the server, we handle the message here
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
                    case CommandBase.ConnectionResultCommandId:
                        // We only want this message while connecting
                        if (Status != ClientStatus.Connecting)
                            break;

                        // Get the result
                        var connectionResult = ConnectionResultCommand.Deserialize(message);

                        if (connectionResult.Success)
                        {
                            // Log and set that we are connected.
                            CSM.Log($"Successfully connected to server at {peer.EndPoint.Host}:{peer.EndPoint.Port}.");
                            Status = ClientStatus.Connected;
                        }
                        else
                        {
                            CSM.Log($"Could not connect to server at {peer.EndPoint.Host}:{peer.EndPoint.Port}. Disconnecting... Error Message: {connectionResult.Reason}");
                            ConnectionMessage = $"Could not connect to server at {peer.EndPoint.Host}:{peer.EndPoint.Port}. Disconnecting... Error Message: {connectionResult.Reason}";
                            Disconnect();
                        }
                        break;
                    // Handle ping commands by returning the ping
                    case CommandBase.PingCommandId:
                        // Update the last server ping
                        _lastServerPing = DateTime.UtcNow;
                        // Send back a ping event
                        peer.Send(ArrayHelpers.PrependByte(CommandBase.PingCommandId, new PingCommand().Serialize()), SendOptions.ReliableOrdered);
                        break;
                    case CommandBase.SimulationCommandID:
                        var simulation = SimulationCommand.Deserialize(message);

                        SimulationManager.instance.SimulationPaused = simulation.SimulationPaused;
                        SimulationManager.instance.SelectedSimulationSpeed = simulation.SelectedSimulationSpeed;
                        SimulationManager.instance.ForcedSimulationPaused = simulation.ForcedSimulationPaused;
                        break;
                }
            } 
            catch (Exception ex)
            {
                CSM.Log($"Received an error from {peer.EndPoint.Host}:{peer.EndPoint.Port}. Message: {ex.Message}");
            }
        }

        /// <summary>
        ///     Called once we have connected to the server,
        ///     at this point we want to send a connect request packet 
        ///     to the server
        /// </summary>
        private void ListenerOnPeerConnectedEvent(NetPeer peer)
        {
            // Build the connection request
            var connectionRequest = new ConnectionRequestCommand
            {
                GameVersion = CSM.IsUnity ? BuildConfig.applicationVersion : "1.0.0",
                ModCount = CSM.IsUnity ? PluginManager.instance.modCount : 0,
                ModVersion = Assembly.GetAssembly(typeof(Client)).GetName().Version.ToString(),
                Password = _clientConfig.Password,
                Username = _clientConfig.Username
            };

            // Send the message
            peer.Send(ArrayHelpers.PrependByte(CommandBase.ConnectionRequestCommandId, connectionRequest.Serialize()), SendOptions.ReliableOrdered);
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
using LiteNetLib;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CSM.GS.Commands;
using CSM.GS.Commands.Data.ApiServer;
using Microsoft.Extensions.Configuration;

namespace CSM.GS
{
    /// <summary>
    ///     This background service a UDP server which matches
    ///     servers and clients up via NAT hole punching.
    ///
    ///     TODO:
    ///         - Let clients connect using a token instead of an IP address
    /// </summary>
    public class WorkerService : BackgroundService, INatPunchListener
    {
        // Constants
        public int ServerPort => int.TryParse(_config.GetSection("PORT").Value, out int val) ? val : 4240;
        private int ServerTick => int.TryParse(_config.GetSection("TICK").Value, out int val) ? val : 10;
        private TimeSpan KickTime => TimeSpan.FromSeconds(int.TryParse(_config.GetSection("KICK_TIME").Value, out int val) ? val : 15);
        private IPAddress LocalServerAddress => IPAddress.TryParse(_config.GetSection("LOCAL_IP").Value, out IPAddress val) ? val : IPAddress.Parse("127.0.0.1");
        public IPAddress RemoteServerAddress => IPAddress.TryParse(_config.GetSection("SERVER_IP").Value, out IPAddress val) ? val : IPAddress.Parse("127.0.0.1");

        private NetManager _puncher;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        private readonly Dictionary<IPAddress, Server> _gameServers = new();
        private readonly List<IPAddress> _serversToRemove = new();

        private readonly ConcurrentQueue<Action> _tasks = new();

        private readonly Type _responsePacketType;
        private readonly object _responsePacket;
        private readonly MethodInfo _responsePacketSend;

        public WorkerService(ILogger<WorkerService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            ApiCommand.Instance = new ApiCommand();
            ApiCommand.Instance.RefreshModel();

            _responsePacketType = typeof(NatPunchModule).GetNestedType("NatIntroduceResponsePacket", BindingFlags.NonPublic);
            if (_responsePacketType != null)
            {
                _responsePacket = Activator.CreateInstance(_responsePacketType);
                MethodInfo send =
                    typeof(NatPunchModule).GetMethod("Send", BindingFlags.NonPublic | BindingFlags.Instance);
                if (send != null) _responsePacketSend = send.MakeGenericMethod(_responsePacketType);
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            EventBasedNetListener netListener = new();

            // Here we update the last contact time in the list of internal servers
            netListener.NetworkReceiveUnconnectedEvent += (point, reader, type) =>
            {
                if (type != UnconnectedMessageType.BasicMessage)
                    return;

                try
                {
                    CommandReceiver.Parse(this, point, reader);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Encountered an error while reading command from {Address}:{Port}:", Anonymize(point.Address), point.Port);
                }
            };

            _puncher = new NetManager(netListener);

            _puncher.Start(ServerPort);
            _puncher.NatPunchEnabled = true;
            _puncher.UnconnectedMessagesEnabled = true;
            _puncher.NatPunchModule.Init(this);

            _logger.LogInformation("Starting NAT Relay Server...");

            // Loop
            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime now = DateTime.Now;

                // Run queued actions in this thread
                while (_tasks.TryDequeue(out Action action))
                    action.Invoke();

                _puncher.NatPunchModule.PollEvents();
                _puncher.PollEvents();

                // Check old servers, if a server has been waiting for longer than
                // "KickTime", they need to be removed from the server list
                foreach (KeyValuePair<IPAddress, Server> server in _gameServers)
                {
                    if (now - server.Value.LastPing > KickTime)
                    {
                        _serversToRemove.Add(server.Key);
                    }
                }

                // Now actually remove servers that are due for removal
                foreach (IPAddress ip in _serversToRemove)
                {
                    _logger.LogInformation("[{ExternalAddress}] Server has disconnected, removing from internal dictionary...", Anonymize(ip));
                    _gameServers.Remove(ip);
                }

                _serversToRemove.Clear();

                Thread.Sleep(ServerTick);
            }

            _logger.LogInformation("Stopping NAT Relay Server...");

            _puncher.Stop();
            return Task.CompletedTask;
        }

        public void OnNatIntroductionRequest(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint, string token)
        {
            // If this is the local IP of this server, we are connecting from the port check client
            bool isFromPortTester = Equals(remoteEndPoint.Address, LocalServerAddress);
            if (isFromPortTester)
            {
                remoteEndPoint.Address = RemoteServerAddress;
            }

            IPAddress serverIp;
            try
            {
                serverIp = IPAddress.Parse(token);
            }
            catch (FormatException)
            {
                return;
            }

            if (_gameServers.TryGetValue(serverIp, out Server server))
            {
                // At this point, we have access to the client and server, we can now introduce them
                _logger.LogInformation("Introduction -> Host: {HostingInternalAddress} {HostExternalAddress}, Client: {ClientInternalAddress} {ClientExternalAddress}", Anonymize(server.InternalAddress), Anonymize(server.ExternalAddress), Anonymize(localEndPoint), Anonymize(remoteEndPoint));

                // The following is a reproduction of NatPunchModule::NatIntroduce, but changes the client remote address if it comes from our port checker
                _responsePacketType?.GetProperty("Token")?.SetValue(_responsePacket, token);

                _responsePacketType?.GetProperty("Internal")?.SetValue(_responsePacket, server.InternalAddress);
                _responsePacketType?.GetProperty("External")?.SetValue(_responsePacket, server.ExternalAddress);
                _responsePacketSend?.Invoke(_puncher.NatPunchModule, new [] {_responsePacket, isFromPortTester ? localEndPoint : remoteEndPoint});

                _responsePacketType?.GetProperty("Internal")?.SetValue(_responsePacket, localEndPoint);
                _responsePacketType?.GetProperty("External")?.SetValue(_responsePacket, remoteEndPoint);
                _responsePacketSend?.Invoke(_puncher.NatPunchModule, new [] {_responsePacket, server.ExternalAddress});
            }
            else
            {
                _logger.LogInformation("Server not found, ignoring...");
            }
        }

        public void OnNatIntroductionSuccess(IPEndPoint targetEndPoint, NatAddressType type, string token)
        {
            // Ignore as we are the API server
        }

        public void RegisterServer(IPEndPoint remoteEndPoint, IPEndPoint localEndPoint, string token)
        {
            if (!_gameServers.ContainsKey(remoteEndPoint.Address))
            {
                _logger.LogInformation("[{ExternalAddress}] Registered Server: Internal Address={InternalAddress} Token={Token}", Anonymize(remoteEndPoint), Anonymize(localEndPoint), token);
            }
            // Always create new server entry, so that port numbers and the token are updated
            _gameServers[remoteEndPoint.Address] = new Server(localEndPoint, remoteEndPoint, token);
        }

        public void SendToServer(IPEndPoint server, ApiCommandBase command)
        {
            byte[] data = ApiCommand.Serialize(command);
            _puncher.SendUnconnectedMessage(data, server);
        }

        private static string Anonymize(IPEndPoint endpoint)
        {
            return Anonymize(endpoint.Address) + ":" + endpoint.Port;
        }

        private static string Anonymize(IPAddress address)
        {
            string[] parts = address.ToString().Split('.');
            if (parts.Length == 4)
            {
                return parts[0] + '.' + parts[1] + ".x.x";
            }
            else
            {
                return address.ToString();
            }
        }

        public void QueueAction(Action action)
        {
            _tasks.Enqueue(action);
        }
    }
}

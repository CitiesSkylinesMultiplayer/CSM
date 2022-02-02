using CSM.Networking.Config;
using CSM.Networking.Status;

namespace CSM.Networking
{
    /// <summary>
    ///     Server
    /// </summary>
    public class Server : BaseServer
    {
        /// <summary>
        ///     Get the Player object of the server host
        /// </summary>
        public Player HostPlayer { get { return _hostPlayer; } }
        // The player instance for the host player
        private Player _hostPlayer;

        public Server() : base()
        {

        }

        /// <summary>
        ///     Starts the server with the specified config options
        /// </summary>
        /// <param name="serverConfig">Server config information</param>
        /// <returns>If the server has started.</returns>
        public override bool StartServer(ServerConfig serverConfig)
        {
            if(base.StartServer(serverConfig))
            {
                _hostPlayer = new Player(Config.Username);
                _hostPlayer.Status = ClientStatus.Connected;
                MultiplayerManager.Instance.PlayerList.Add(_hostPlayer.Username);

                return true;
            }
            return false;
        }

        /// <summary>
        ///     Get the Player object by username. Warning, expensive call!!!
        /// </summary>
        public override Player GetPlayerByUsername(string username)
        {
            if (username == HostPlayer.Username)
                return HostPlayer;
            else
                return base.GetPlayerByUsername(username);
        }
    }
}

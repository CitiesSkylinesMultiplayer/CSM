using CSM.API.Networking.Status;
using LiteNetLib;

namespace CSM.Networking
{
    public class Player : global::CSM.API.Networking.Player
    {
        // Username is inherited

        public NetPeer NetPeer { get; set; }

        // Status and Latency are inherited

        public Player(NetPeer peer, string username)
        {
            Username = username;
            NetPeer = peer;
            Latency = -1;
        }

        public Player(string username) : this(null, username)
        {
        }

        public Player() : this(null, null)
        {
        }
    }
}

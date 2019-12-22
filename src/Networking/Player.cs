using CSM.Networking.Status;
using LiteNetLib;

namespace CSM.Networking
{
    public class Player
    {
        public string Username { get; set; }

        public NetPeer NetPeer { get; set; }

        public long Latency { get; set; }

        public ClientStatus Status { get; set; }

        public Player(NetPeer peer, string username)
        {
            Username = username;
            NetPeer = peer;
            Latency = -1;
        }

        public Player(string username) : this(null, username)
        {
        }
    }
}

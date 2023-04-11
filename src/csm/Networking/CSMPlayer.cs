using CSM.API.Networking;
using LiteNetLib;

namespace CSM.Networking
{
    public class CSMPlayer : Player
    {
        public NetPeer NetPeer { get; }

        public CSMPlayer(NetPeer peer, string username) : base(username)
        {
            NetPeer = peer;
        }

        public CSMPlayer(string username) : base(username)
        {
            NetPeer = null;
        }
    }
}

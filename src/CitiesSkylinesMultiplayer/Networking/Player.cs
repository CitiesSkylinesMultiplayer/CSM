using LiteNetLib;

namespace CitiesSkylinesMultiplayer.Networking
{
    public class Player
    {
        public string Username { get; set; }

        public NetPeer NetPeer { get; set; }
    }
}
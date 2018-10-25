using LiteNetLib;
using System;

namespace CSM.Networking
{
    public class Player
    {
        public string Username { get; set; }

        public NetPeer NetPeer { get; set; }

        public DateTime LastPing { get; set; }

        public Player(NetPeer peer, string username)
        {
            Username = username;
            NetPeer = peer;
            LastPing = DateTime.UtcNow;
        }

        public Player(string username) : this(null, username)
        {
        }
    }
}
using System;
using CSM.Networking;

namespace CSM.Events
{
    public class PlayerEventArgs : EventArgs
    {
        public Player Player { get; private set; }

        public PlayerEventArgs(Player player)
        {
            Player = player;
        }
    }
}

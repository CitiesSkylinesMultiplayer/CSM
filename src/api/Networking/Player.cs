using CSM.API.Networking.Status;

namespace CSM.API.Networking
{
    public class Player
    {
        public string Username { get; set; }

        public long Latency { get; set; }

        public ClientStatus Status { get; set; }

        public Player(string username)
        {
            Username = username;
            Latency = -1;
        }

        public Player()
        {
        }
    }
}

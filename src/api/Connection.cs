using CSM.API.Commands;
using System;

namespace CSM.API
{
    public class Connection
    {
        private static Connection instance;
        public string name { get; set; }
        public Func<CommandBase, bool> SentToAll, SendToServer;

        public static Connection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Connection();
                }

                return instance;
            }
        }

        public bool ConnectToCSM(Func<CommandBase, bool> SentToAll, Func<CommandBase, bool> SendToServer)
        {
            this.SentToAll = SentToAll;
            this.SendToServer = SendToServer;
            return true;
        }
    }
}

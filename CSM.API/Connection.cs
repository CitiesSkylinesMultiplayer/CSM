using System;
using CSM.API.Commands;

namespace CSM.API
{
    public class Connection
    {
        private static Connection instance;
        public string name;
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

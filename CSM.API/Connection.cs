using System;
using CSM.API.Commands;

namespace CSM.API
{
    public abstract class Connection
    {
        private static Connection instance;
        public string name;
        public Func<CommandBase, bool> SentToAll;

        public static Connection Instance
        {
            get
            {
                return instance;
            }
        }

        public static void SetInstance(Connection conn)
        {
            instance = conn;
        }

        public bool ConnectToCSM(Func<CommandBase, bool> SendToAllFunction)
        {
            SentToAll = SendToAllFunction;
            SentToAll(new ConnectionTestCommand
                { connectionTestInt = 1 }
            );
            return true;
        }
    }
}

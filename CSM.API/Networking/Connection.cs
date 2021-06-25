using System;
using System.Diagnostics.PerformanceData;
using CSM.API.Commands;
using LiteNetLib;

namespace CSM.API.Networking
{
    public class Connection : IConnection
    {
        private static Connection instance;

        private Func<NetPeer, CommandBase, bool> sendToClient;
        private Func<Player, CommandBase, bool> sendToClient2;
        private Func<CommandBase, bool> sendToClients;
        private Func<CommandBase, Player, bool> sendToOtherClients;
        private Func<CommandBase, bool> sendToServer;
        private Func<CommandBase, bool> sendToAll;

        public static Connection Instance {  
            get {  
                return instance;  
            }  
        }

        public static void SetInstance(Connection conn)
        {
            instance = conn;
        }

        public void ConnectToCSM(Func<NetPeer, CommandBase, bool> sendToClient,
            Func<Player, CommandBase, bool> sendToClient2, Func<CommandBase, bool> sendToClients,
            Func<CommandBase, Player, bool> sendToOtherClients, Func<CommandBase, bool> sendToServer,
            Func<CommandBase, bool> sendToAll)
        {
            this.sendToClient = sendToClient;
            this.sendToClient2 = sendToClient2;
            this.sendToClients = sendToClients;
            this.sendToOtherClients = sendToOtherClients;
            this.sendToServer = sendToServer;
            this.sendToAll = sendToAll;
        }

        public void SendToClient(NetPeer peer, CommandBase command)
        {
            sendToClient(peer, command);
        }

        public void SendToClient(Player player, CommandBase command)
        {
            sendToClient2(player, command);
        }

        public void SendToClients(CommandBase command)
        {
            sendToClients(command);
        }

        public void SendToOtherClients(CommandBase command, Player exclude)
        {
            sendToOtherClients(command, exclude);
        }

        public void SendToServer(CommandBase command)
        {
            sendToServer(command);
        }

        public void SendToAll(CommandBase command)
        {
            sendToAll(command);
        }
    }
}
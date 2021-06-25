using System;
using CSM.API.Commands;
using LiteNetLib;

namespace CSM.API.Networking
{
    public interface IConnection
    {
        void ConnectToCSM(Func<NetPeer, CommandBase, bool> sendToClient,
            Func<Player, CommandBase, bool> sendToClient2, Func<CommandBase, bool> sendToClients,
            Func<CommandBase, Player, bool> sendToOtherClients, Func<CommandBase, bool> sendToServer,
            Func<CommandBase, bool> sendToAll);
        void SendToClient(NetPeer peer, CommandBase command);
        void SendToClient(Player player, CommandBase command);
        void SendToClients(CommandBase command);
        void SendToOtherClients(CommandBase command, Player exclude);
        void SendToServer(CommandBase command);
        void SendToAll(CommandBase command);
    }
}
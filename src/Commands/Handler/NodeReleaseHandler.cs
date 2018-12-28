using ColossalFramework;
using CSM.Injections;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class NodeReleaseHandler : CommandHandler<NodeReleaseCommand>
    {
        public NodeReleaseHandler()
        {
            Transaction = TransactionType.NODES;
        }

        public override byte ID => CommandIds.NodeReleaseCommand;

        public override void HandleOnServer(NodeReleaseCommand command, Player player) => Handle(command);

        public override void HandleOnClient(NodeReleaseCommand command) => Handle(command);

        private void Handle(NodeReleaseCommand command)
        {
            NodeHandler.IgnoreNodes.Add(command.NodeId);
            Singleton<NetManager>.instance.ReleaseNode(command.NodeId);
            NodeHandler.IgnoreNodes.Remove(command.NodeId);
        }
    }
}
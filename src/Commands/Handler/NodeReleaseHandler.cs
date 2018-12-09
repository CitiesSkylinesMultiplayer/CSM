using ColossalFramework;
using CSM.Injections;
using CSM.Networking;
using System.Linq;

namespace CSM.Commands.Handler
{
    public class NodeReleaseHandler : CommandHandler<NodeReleaseCommand>
    {
        public override byte ID => 112;

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

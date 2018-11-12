using ColossalFramework;
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
            lock (Extensions.NodeAndSegmentExtension.NodesCreated)
            { 
            Extensions.NodeAndSegmentExtension.NodesCreated.Remove(command.NodeId);
            Singleton<NetManager>.instance.ReleaseNode(command.NodeId);
            }
        }
    }
}
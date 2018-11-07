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
            var NodeID = Extensions.NodeAndSegmentExtension.NodeIDDictionary[command.NodeId];

            foreach (var ID in Extensions.NodeAndSegmentExtension.VectorDictionary.Where(kvp => kvp.Value == NodeID).ToList())
            {
                Extensions.NodeAndSegmentExtension.VectorDictionary.Remove(ID.Key);
            }
            Extensions.NodeAndSegmentExtension.NodeIDDictionary.Remove(NodeID);
            Singleton<NetManager>.instance.ReleaseNode(NodeID);
        }
    }
}
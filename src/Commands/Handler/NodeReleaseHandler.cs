using ColossalFramework;
using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class NodeReleaseHandler : CommandHandler<NodeReleaseCommand>
    {
        public override void Handle(NodeReleaseCommand command)
        {
            NodeHandler.IgnoreNodes.Add(command.NodeId);
            Singleton<NetManager>.instance.ReleaseNode(command.NodeId);
            NodeHandler.IgnoreNodes.Remove(command.NodeId);
        }
    }
}

using ColossalFramework;
using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class NodeReleaseHandler : CommandHandler<NodeReleaseCommand>
    {
        public override void Handle(NodeReleaseCommand command)
        {
            NodeHandler.IgnoreAll = true;
            Singleton<NetManager>.instance.ReleaseNode(command.NodeId);
            NodeHandler.IgnoreAll = false;
        }
    }
}

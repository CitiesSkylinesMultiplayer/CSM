using CSM.API.Commands;
using CSM.Commands.Data.Net;
using CSM.Helpers;

namespace CSM.Commands.Handler.Net
{
    public class NodeReleaseHandler : CommandHandler<NodeReleaseCommand>
    {
        protected override void Handle(NodeReleaseCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            NetManager.instance.ReleaseNode(command.NodeId);
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

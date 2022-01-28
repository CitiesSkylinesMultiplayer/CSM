using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Net;

namespace CSM.BaseGame.Commands.Handler.Net
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

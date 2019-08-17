using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class TreeReleaseHandler : CommandHandler<TreeReleaseCommand>
    {
        public override void Handle(TreeReleaseCommand command)
        {
            TreeHandler.IgnoreAll = true;
            TreeManager.instance.ReleaseTree(command.TreeId);
            TreeHandler.IgnoreAll = false;
        }
    }
}

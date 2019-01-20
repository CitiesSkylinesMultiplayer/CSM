using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class TreeReleaseHandler : CommandHandler<TreeReleaseCommand>
    {
        public override void Handle(TreeReleaseCommand command)
        {
            TreeHandler.IgnoreTrees.Add(command.TreeID);
            TreeManager.instance.ReleaseTree(command.TreeID);
            TreeHandler.IgnoreTrees.Remove(command.TreeID);
        }
    }
}

using CSM.API.Commands;
using CSM.Commands.Data.Trees;
using CSM.Helpers;

namespace CSM.Commands.Handler.Trees
{
    public class TreeReleaseHandler : CommandHandler<TreeReleaseCommand>
    {
        protected override void Handle(TreeReleaseCommand command)
        {
            IgnoreHelper.StartIgnore();
            TreeManager.instance.ReleaseTree(command.TreeId);
            IgnoreHelper.EndIgnore();
        }
    }
}

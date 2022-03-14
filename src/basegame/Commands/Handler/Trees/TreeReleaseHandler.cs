using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Trees;

namespace CSM.BaseGame.Commands.Handler.Trees
{
    public class TreeReleaseHandler : CommandHandler<TreeReleaseCommand>
    {
        protected override void Handle(TreeReleaseCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            TreeManager.instance.ReleaseTree(command.TreeId);
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

using CSM.API.Commands;
using CSM.Commands.Data.Trees;
using CSM.Helpers;

namespace CSM.Commands.Handler.Trees
{
    public class TreeMoveHandler : CommandHandler<TreeMoveCommand>
    {
        protected override void Handle(TreeMoveCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            TreeManager.instance.MoveTree(command.TreeId, command.Position);
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

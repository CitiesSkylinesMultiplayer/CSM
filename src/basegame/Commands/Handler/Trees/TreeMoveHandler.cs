using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Trees;

namespace CSM.BaseGame.Commands.Handler.Trees
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

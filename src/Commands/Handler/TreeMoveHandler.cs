using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class TreeMoveHandler : CommandHandler<TreeMoveCommand>
    {
        public override void Handle(TreeMoveCommand command)
        {
            TreeHandler.IgnoreAll = true;
            TreeManager.instance.MoveTree(command.TreeId, command.Position);
            TreeHandler.IgnoreAll = false;
        }
    }
}

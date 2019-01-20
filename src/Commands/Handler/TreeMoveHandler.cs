using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class TreeMoveHandler : CommandHandler<TreeMoveCommand>
    {
        public override void Handle(TreeMoveCommand command)
        {
            TreeHandler.IgnoreTrees.Add(command.TreeID);
            TreeManager.instance.MoveTree(command.TreeID, command.Position);
            TreeHandler.IgnoreTrees.Remove(command.TreeID);
        }
    }
}

using CSM.Injections;
using CSM.Networking;



namespace CSM.Commands.Handler
{
    public class TreeMoveHandler : CommandHandler<TreeMoveCommand>
    {
        public override byte ID => CommandIds.TreeMoveCommand;

        public override void HandleOnClient(TreeMoveCommand command) => Handle(command);

        public override void HandleOnServer(TreeMoveCommand command, Player player) => Handle(command);

        private void Handle(TreeMoveCommand command)
        {
            TreeHandler.IgnoreTrees.Add(command.TreeID);
            TreeManager.instance.MoveTree(command.TreeID, command.Position);
            TreeHandler.IgnoreTrees.Remove(command.TreeID);

        }

    }
}

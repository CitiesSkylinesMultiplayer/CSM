using CSM.Injections;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    class TreeReleaseHandler : CommandHandler<TreeReleaseCommand>
    {
        public override byte ID => CommandIds.TreeReleaseCommand;

        public override void HandleOnClient(TreeReleaseCommand command) => Handle(command);

        public override void HandleOnServer(TreeReleaseCommand command, Player player) => Handle(command);

        private void Handle(TreeReleaseCommand command)
        {
            TreeHandler.IgnoreTrees.Add(command.TreeID);
            TreeManager.instance.ReleaseTree(command.TreeID);
            TreeHandler.IgnoreTrees.Remove(command.TreeID);

        }

    }
}

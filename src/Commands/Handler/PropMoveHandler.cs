using CSM.Injections;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    class PropMoveHandler : CommandHandler<PropMoveCommand>
    {
        public override byte ID => CommandIds.PropMoveCommand;

        public override void HandleOnClient(PropMoveCommand command) => Handle(command);

        public override void HandleOnServer(PropMoveCommand command, Player player) => Handle(command);

        private void Handle(PropMoveCommand command)
        {
            PropHandler.IgnoreProp.Add(command.PropID);
            PropManager.instance.MoveProp(command.PropID, command.Position);
            PropHandler.IgnoreProp.Remove(command.PropID);

        }
    }
}

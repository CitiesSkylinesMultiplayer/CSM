using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class PropMoveHandler : CommandHandler<PropMoveCommand>
    {
        public override void Handle(PropMoveCommand command)
        {
            PropHandler.IgnoreAll = true;
            PropManager.instance.MoveProp(command.PropID, command.Position);
            PropHandler.IgnoreAll = false;
        }
    }
}

using CSM.Commands.Data.Props;
using CSM.Helpers;

namespace CSM.Commands.Handler.Props
{
    public class PropMoveHandler : CommandHandler<PropMoveCommand>
    {
        protected override void Handle(PropMoveCommand command)
        {
            IgnoreHelper.StartIgnore();
            PropManager.instance.MoveProp(command.PropId, command.Position);
            IgnoreHelper.EndIgnore();
        }
    }
}

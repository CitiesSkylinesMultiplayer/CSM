using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Props;

namespace CSM.BaseGame.Commands.Handler.Props
{
    public class PropMoveHandler : CommandHandler<PropMoveCommand>
    {
        protected override void Handle(PropMoveCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            PropManager.instance.MoveProp(command.PropId, command.Position);
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

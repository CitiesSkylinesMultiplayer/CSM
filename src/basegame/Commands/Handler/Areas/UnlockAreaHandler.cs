using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Areas;

namespace CSM.BaseGame.Commands.Handler.Areas
{
    public class UnlockAreaHandler : CommandHandler<UnlockAreaCommand>
    {
        protected override void Handle(UnlockAreaCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            int area = (command.Z * 5) + command.X; // Calculate the area index
            GameAreaManager.instance.UnlockArea(area);

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

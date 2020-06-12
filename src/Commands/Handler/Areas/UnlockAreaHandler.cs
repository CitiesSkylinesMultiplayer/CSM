using CSM.Commands.Data.Areas;
using CSM.Helpers;

namespace CSM.Commands.Handler.Areas
{
    public class UnlockAreaHandler : CommandHandler<UnlockAreaCommand>
    {
        protected override void Handle(UnlockAreaCommand command)
        {
            IgnoreHelper.StartIgnore();

            int area = (command.Z * 5) + command.X; // Calculate the area index
            GameAreaManager.instance.UnlockArea(area);

            IgnoreHelper.EndIgnore();
        }
    }
}

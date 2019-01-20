
namespace CSM.Commands.Handler
{
    public class UnlockAreaHandler : CommandHandler<UnlockAreaCommand>
    {
        public override void Handle(UnlockAreaCommand command)
        {
            var area = (command.Z * 5) + command.X; //calculate the index
            GameAreaManager.instance.UnlockArea(area);
        }
    }
}

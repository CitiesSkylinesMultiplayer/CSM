namespace CSM.Commands.Handler
{
    public class UnlockAreaHandler : CommandHandler<UnlockAreaCommand>
    {
        public override void Handle(UnlockAreaCommand command)
        {
            int area = (command.Z * 5) + command.X; //calculate the index
            GameAreaManager.instance.UnlockArea(area);
        }
    }
}

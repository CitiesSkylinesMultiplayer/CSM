using CSM.Extensions;
using CSM.Networking;
using ColossalFramework;

namespace CSM.Commands.Handler
{
    public class UnlockAreaHandler : CommandHandler<UnlockAreaCommand>
    {
        public override byte ID => CommandIds.UnlockAreaCommand;

        public override void HandleOnServer(UnlockAreaCommand command, Player player) => Handle(command);

        public override void HandleOnClient(UnlockAreaCommand command) => Handle(command);

        private void Handle(UnlockAreaCommand command)
        {

            var area = (command.Z * 5) + command.X; //calculate the index            
            GameAreaManager.instance.m_areaGrid[area] = ++GameAreaManager.instance.m_areaCount;


        }
    }
}

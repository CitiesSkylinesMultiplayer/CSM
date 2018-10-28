
using CSM.Networking;

namespace CSM.Commands.Handler
{
    class WorldInfoHandler : CommandHandler<WorldInfoCommand>
    {
        public override byte ID => 53;

        public override void HandleOnClient(WorldInfoCommand command)
        {
            SimulationManager.instance.m_currentGameTime = command.CurrentGameTime;
            SimulationManager.instance.m_currentDayTimeHour = command.CurrentDayTimeHour;
        }

        public override void HandleOnServer(WorldInfoCommand command, Player player) { }

        public override void OnClientConnect(Player player)
        {
            // Send the world info command, sets up the players world
            Command.SendToClient(player, new WorldInfoCommand { CurrentGameTime = SimulationManager.instance.m_currentGameTime, CurrentDayTimeHour = SimulationManager.instance.m_currentDayTimeHour });
        }
    }
}

using CSM.Injections;
using CSM.Panels;
using NLog;

namespace CSM.Commands.Handler
{
    public class ParkCreateHandler : CommandHandler<ParkCreateCommand>
    {
        public override void Handle(ParkCreateCommand command)
        {
            DistrictHandler.IgnoreAll = true;
            DistrictManager.instance.CreatePark(out byte park, command.ParkType, command.ParkLevel);
            
            if (park != command.ParkID)
            {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, $"Park array no longer in sync! Generated {park} instead of {command.ParkID}");
                ChatLogPanel.PrintGameMessage(ChatLogPanel.MessageType.Error, "Park array no longer in sync! Please restart the multiplayer session!");
            }

            DistrictManager.instance.m_parks.m_buffer[park].m_randomSeed = command.Seed;
            DistrictHandler.IgnoreAll = false;
        }
    }
}

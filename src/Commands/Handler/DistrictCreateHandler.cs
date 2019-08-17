using CSM.Injections;
using CSM.Panels;
using NLog;

namespace CSM.Commands.Handler
{
    public class DistrictCreateHandler : CommandHandler<DistrictCreateCommand>
    {
        public override void Handle(DistrictCreateCommand command)
        {
            DistrictHandler.IgnoreAll = true;
            DistrictManager.instance.CreateDistrict(out byte district);

            if (district != command.DistrictId)
            {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, $"District array no longer in sync! Generated {district} instead of {command.DistrictId}");
                ChatLogPanel.PrintGameMessage(ChatLogPanel.MessageType.Error, "District array no longer in sync! Please restart the multiplayer session!");
            }

            DistrictManager.instance.m_districts.m_buffer[district].m_randomSeed = command.Seed;
            DistrictHandler.IgnoreAll = false;
        }
    }
}

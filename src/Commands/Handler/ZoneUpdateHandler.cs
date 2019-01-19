using ColossalFramework;
using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class ZoneUpdateHandler : CommandHandler<ZoneUpdateCommand>
    {
        public override void Handle(ZoneUpdateCommand command)
        {
            Singleton<ZoneManager>.instance.m_blocks.m_buffer[command.ZoneId].m_zone1 = command.Zone1;
            Singleton<ZoneManager>.instance.m_blocks.m_buffer[command.ZoneId].m_zone2 = command.Zone2;

            ZoneHandler.IgnoreZones.Add(command.ZoneId);
            // This Command is necessary to get the Zone to render, else it will first show when a building is build on it.
            Singleton<ZoneManager>.instance.m_blocks.m_buffer[command.ZoneId].RefreshZoning(command.ZoneId);
            ZoneHandler.IgnoreZones.Remove(command.ZoneId);
        }
    }
}

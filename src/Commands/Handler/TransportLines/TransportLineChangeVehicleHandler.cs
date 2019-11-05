using CSM.Commands.Data.TransportLines;
using CSM.Helpers;

namespace CSM.Commands.Handler.TransportLines
{
    public class TransportLineChangeVehicleHandler : CommandHandler<TransportLineChangeVehicleCommand>
    {
        protected override void Handle(TransportLineChangeVehicleCommand command)
        {
            IgnoreHelper.StartIgnore();

            // Use ref because otherwise the TransportLine struct(!) would be copied
            ref TransportLine line = ref TransportManager.instance.m_lines.m_buffer[command.LineId];
            VehicleInfo vehicle = PrefabCollection<VehicleInfo>.GetPrefab(command.Vehicle);

            ReflectionHelper.Call(line, "ReplaceVehicles", command.LineId, vehicle);

            IgnoreHelper.EndIgnore();
        }
    }
}

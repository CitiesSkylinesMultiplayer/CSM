using CSM.API.Commands;
using CSM.Commands.Data.TransportLines;
using CSM.Helpers;

namespace CSM.Commands.Handler.TransportLines
{
    public class TransportLineChangeVehicleHandler : CommandHandler<TransportLineChangeVehicleCommand>
    {
        protected override void Handle(TransportLineChangeVehicleCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            // Use ref because otherwise the TransportLine struct(!) would be copied
            ref TransportLine line = ref TransportManager.instance.m_lines.m_buffer[command.LineId];

            VehicleInfo vehicle = null;
            if (command.Vehicle.HasValue) {
                vehicle = PrefabCollection<VehicleInfo>.GetPrefab(command.Vehicle.Value);
            }

            ReflectionHelper.Call(line, "ReplaceVehicles", command.LineId, vehicle);

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

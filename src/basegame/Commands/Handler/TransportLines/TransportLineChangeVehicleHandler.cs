using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.TransportLines;

namespace CSM.BaseGame.Commands.Handler.TransportLines
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

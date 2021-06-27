using ColossalFramework.UI;
using CSM.API.Commands;
using CSM.Commands.Data.TransportLines;
using CSM.Helpers;

namespace CSM.Commands.Handler.TransportLines
{
    public class TransportLineChangeSliderHandler : CommandHandler<TransportLineChangeSliderCommand>
    {
        protected override void Handle(TransportLineChangeSliderCommand command)
        {
            IgnoreHelper.StartIgnore();

            TransportLine[] lines = TransportManager.instance.m_lines.m_buffer;
            if (command.IsTicketPrice)
                lines[command.LineId].m_ticketPrice = (ushort)command.Value;
            else
                lines[command.LineId].m_budget = (ushort)command.Value;

            // Update info panel if open:
            PublicTransportWorldInfoPanel panel = UIView.library.Get<PublicTransportWorldInfoPanel>(typeof(PublicTransportWorldInfoPanel).Name);
            ushort lineId = ReflectionHelper.Call<ushort>(panel, "GetLineID");
            if (lineId == command.LineId)
            {
                UISlider slider = ReflectionHelper.GetAttr<UISlider>(panel, command.IsTicketPrice ? "m_ticketPriceSlider" : "m_vehicleCountModifier");

                if (slider != null)
                {
                    SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                    {
                        slider.value = command.Value;
                    });
                }
            }

            IgnoreHelper.EndIgnore();
        }
    }
}

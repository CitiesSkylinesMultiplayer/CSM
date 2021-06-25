using CSM.API.Commands;
using CSM.Commands.Data.TransportLines;
using CSM.Helpers;

namespace CSM.Commands.Handler.TransportLines
{
    public class TransportLineChangeActiveHandler : CommandHandler<TransportLineChangeActiveCommand>
    {
        protected override void Handle(TransportLineChangeActiveCommand command)
        {
            IgnoreHelper.StartIgnore();
            TransportManager.instance.m_lines.m_buffer[command.LineId].SetActive(command.Day, command.Night);
            IgnoreHelper.EndIgnore();
        }
    }
}

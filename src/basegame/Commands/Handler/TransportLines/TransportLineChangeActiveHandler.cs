using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.TransportLines;

namespace CSM.BaseGame.Commands.Handler.TransportLines
{
    public class TransportLineChangeActiveHandler : CommandHandler<TransportLineChangeActiveCommand>
    {
        protected override void Handle(TransportLineChangeActiveCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            TransportManager.instance.m_lines.m_buffer[command.LineId].SetActive(command.Day, command.Night);
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

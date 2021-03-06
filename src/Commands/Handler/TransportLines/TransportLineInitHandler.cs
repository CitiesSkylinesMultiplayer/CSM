using CSM.Commands.Data.TransportLines;
using CSM.Helpers;

namespace CSM.Commands.Handler.TransportLines
{
    public class TransportLineInitHandler : CommandHandler<TransportLineInitCommand>
    {
        protected override void Handle(TransportLineInitCommand command)
        {
            TransportTool tool = ToolSimulator.GetTool<TransportTool>(command.SenderId);

            ReflectionHelper.SetAttr(tool, "m_errors", ToolBase.ToolErrors.Pending);
            ReflectionHelper.SetAttr(tool, "m_lastMoveIndex", -2);
            ReflectionHelper.SetAttr(tool, "m_lastAddIndex", -2);
        }
    }
}

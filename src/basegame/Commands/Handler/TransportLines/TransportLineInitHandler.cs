using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.TransportLines;
using CSM.BaseGame.Helpers;
using ColossalFramework;

namespace CSM.BaseGame.Commands.Handler.TransportLines
{
    public class TransportLineInitHandler : CommandHandler<TransportLineInitCommand>
    {
        protected override void Handle(TransportLineInitCommand command)
        {
            TransportTool tool = Singleton<ToolSimulator>.instance.GetTool<TransportTool>(command.SenderId);

            ReflectionHelper.SetAttr(tool, "m_errors", ToolBase.ToolErrors.Pending);
            ReflectionHelper.SetAttr(tool, "m_lastMoveIndex", -2);
            ReflectionHelper.SetAttr(tool, "m_lastAddIndex", -2);
        }
    }
}

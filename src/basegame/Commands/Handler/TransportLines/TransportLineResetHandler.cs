using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.TransportLines;
using CSM.BaseGame.Helpers;
using CSM.BaseGame.Injections;
using ColossalFramework;

namespace CSM.BaseGame.Commands.Handler.TransportLines
{
    public class TransportLineResetHandler : CommandHandler<TransportLineResetCommand>
    {
        protected override void Handle(TransportLineResetCommand command)
        {
            TransportTool tool = Singleton<ToolSimulator>.instance.GetTool<TransportTool>(command.SenderId);

            ArrayHandler.StartApplying(command.Array16Ids, null);

            IgnoreHelper.Instance.StartIgnore();

            ReflectionHelper.Call(tool, "ResetTool");

            IgnoreHelper.Instance.EndIgnore();

            ArrayHandler.StopApplying();
        }
    }
}

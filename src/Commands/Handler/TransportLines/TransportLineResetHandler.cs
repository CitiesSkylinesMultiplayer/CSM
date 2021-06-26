using CSM.API.Commands;
using CSM.Commands.Data.TransportLines;
using CSM.Helpers;
using CSM.Injections;

namespace CSM.Commands.Handler.TransportLines
{
    public class TransportLineResetHandler : CommandHandler<TransportLineResetCommand>
    {
        protected override void Handle(TransportLineResetCommand command)
        {
            TransportTool tool = ToolSimulator.GetTool<TransportTool>(command.SenderId);

            ArrayHandler.StartApplying(command.Array16Ids, null);

            IgnoreHelper.Instance.StartIgnore();

            ReflectionHelper.Call(tool, "ResetTool");

            IgnoreHelper.Instance.EndIgnore();

            ArrayHandler.StopApplying();
        }
    }
}

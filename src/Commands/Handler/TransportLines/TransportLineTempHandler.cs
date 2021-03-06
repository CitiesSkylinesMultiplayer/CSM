using CSM.Commands.Data.TransportLines;
using CSM.Helpers;
using CSM.Injections;

namespace CSM.Commands.Handler.TransportLines
{
    public class TransportLineTempHandler : CommandHandler<TransportLineTempCommand>
    {
        protected override void Handle(TransportLineTempCommand command)
        {
            TransportTool tool = ToolSimulator.GetTool<TransportTool>(command.SenderId);

            ArrayHandler.StartApplying(command.Array16Ids, null);

            TransportInfo info = PrefabCollection<TransportInfo>.GetPrefab(command.InfoIndex);

            IgnoreHelper.StartIgnore();

            ReflectionHelper.Call(tool, "EnsureTempLine", info, command.SourceLine, command.MoveIndex, command.AddIndex, command.AddPos, command.FixedPlatform);

            IgnoreHelper.EndIgnore();

            ArrayHandler.StopApplying();
        }
    }
}

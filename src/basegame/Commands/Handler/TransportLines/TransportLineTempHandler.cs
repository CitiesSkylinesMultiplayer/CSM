using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.TransportLines;
using CSM.BaseGame.Helpers;
using CSM.BaseGame.Injections;

namespace CSM.BaseGame.Commands.Handler.TransportLines
{
    public class TransportLineTempHandler : CommandHandler<TransportLineTempCommand>
    {
        protected override void Handle(TransportLineTempCommand command)
        {
            TransportTool tool = ToolSimulator.GetTool<TransportTool>(command.SenderId);

            ArrayHandler.StartApplying(command.Array16Ids, null);

            TransportInfo info = PrefabCollection<TransportInfo>.GetPrefab(command.InfoIndex);

            IgnoreHelper.Instance.StartIgnore();

            ReflectionHelper.Call(tool, "EnsureTempLine", info, command.SourceLine, command.MoveIndex, command.AddIndex, command.AddPos, command.FixedPlatform);

            IgnoreHelper.Instance.EndIgnore();

            ArrayHandler.StopApplying();
        }
    }
}

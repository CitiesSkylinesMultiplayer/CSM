using CSM.API.Commands;
using CSM.Commands.Data.TransportLines;
using CSM.Helpers;
using CSM.Injections;
using System.Collections;

namespace CSM.Commands.Handler.TransportLines
{
    public class TransportLineStartEditBuildingHandler : CommandHandler<TransportLineStartEditBuildingCommand>
    {
        protected override void Handle(TransportLineStartEditBuildingCommand command)
        {
            TransportTool tool = ToolSimulator.GetTool<TransportTool>(command.SenderId);

            TransportInfo info = PrefabCollection<TransportInfo>.GetPrefab(command.Prefab);

            ArrayHandler.StartApplying(command.Array16Ids, null);

            IgnoreHelper.Instance.StartIgnore();

            IEnumerator cancelPrevStop = (IEnumerator)ReflectionHelper.Call(tool, "StartEditingBuildingLine", info, command.Building);
            cancelPrevStop.MoveNext();

            IgnoreHelper.Instance.EndIgnore();

            ArrayHandler.StopApplying();
        }
    }
}

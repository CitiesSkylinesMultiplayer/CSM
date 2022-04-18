using System.Collections;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.TransportLines;
using CSM.BaseGame.Helpers;
using CSM.BaseGame.Injections;
using ColossalFramework;

namespace CSM.BaseGame.Commands.Handler.TransportLines
{
    public class TransportLineStartEditBuildingHandler : CommandHandler<TransportLineStartEditBuildingCommand>
    {
        protected override void Handle(TransportLineStartEditBuildingCommand command)
        {
            TransportTool tool = Singleton<ToolSimulator>.instance.GetTool<TransportTool>(command.SenderId);

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

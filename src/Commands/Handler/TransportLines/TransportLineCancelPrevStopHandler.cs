using CSM.Commands.Data.TransportLines;
using CSM.Helpers;
using CSM.Injections;
using System.Collections;

namespace CSM.Commands.Handler.TransportLines
{
    public class TransportLineCancelPrevStopHandler : CommandHandler<TransportLineCancelPrevStopCommand>
    {
        protected override void Handle(TransportLineCancelPrevStopCommand command)
        {
            TransportTool tool = ToolSimulator.GetTool<TransportTool>(command.SenderId);

            tool.m_prefab = PrefabCollection<TransportInfo>.GetPrefab(command.Prefab);
            ReflectionHelper.SetAttr(tool, "m_building", command.Building);

            ArrayHandler.StartApplying(command.Array16Ids, null);

            IgnoreHelper.StartIgnore();

            int mode = ReflectionHelper.GetEnumValue(typeof(TransportTool).GetNestedType("Mode", ReflectionHelper.AllAccessFlags), "AddStops");
            ReflectionHelper.SetAttr(tool, "m_mode", mode);

            IEnumerator cancelPrevStop = (IEnumerator)ReflectionHelper.Call(tool, "CancelPrevStop");
            cancelPrevStop.MoveNext();

            IgnoreHelper.EndIgnore();

            ArrayHandler.StopApplying();
        }
    }
}

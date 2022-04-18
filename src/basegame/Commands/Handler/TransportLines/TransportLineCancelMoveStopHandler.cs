using System.Collections;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.TransportLines;
using CSM.BaseGame.Helpers;
using CSM.BaseGame.Injections;
using ColossalFramework;

namespace CSM.BaseGame.Commands.Handler.TransportLines
{
    public class TransportLineCancelMoveStopHandler : CommandHandler<TransportLineCancelMoveStopCommand>
    {
        protected override void Handle(TransportLineCancelMoveStopCommand command)
        {
            TransportTool tool = Singleton<ToolSimulator>.instance.GetTool<TransportTool>(command.SenderId);

            tool.m_prefab = PrefabCollection<TransportInfo>.GetPrefab(command.Prefab);

            ArrayHandler.StartApplying(command.Array16Ids, null);

            IgnoreHelper.Instance.StartIgnore();

            int mode = ReflectionHelper.GetEnumValue(typeof(TransportTool).GetNestedType("Mode", ReflectionHelper.AllAccessFlags), "MoveStops");
            ReflectionHelper.SetAttr(tool, "m_mode", mode);

            IEnumerator cancelMoveStop = (IEnumerator)ReflectionHelper.Call(tool, "CancelMoveStop");
            cancelMoveStop.MoveNext();

            IgnoreHelper.Instance.EndIgnore();

            ArrayHandler.StopApplying();
        }
    }
}

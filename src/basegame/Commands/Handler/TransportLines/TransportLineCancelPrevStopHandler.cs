using System.Collections;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.TransportLines;
using CSM.BaseGame.Helpers;
using CSM.BaseGame.Injections;
using ColossalFramework;

namespace CSM.BaseGame.Commands.Handler.TransportLines
{
    public class TransportLineCancelPrevStopHandler : CommandHandler<TransportLineCancelPrevStopCommand>
    {
        protected override void Handle(TransportLineCancelPrevStopCommand command)
        {
            TransportTool tool = Singleton<ToolSimulator>.instance.GetTool<TransportTool>(command.SenderId);

            tool.m_prefab = PrefabCollection<TransportInfo>.GetPrefab(command.Prefab);
            ReflectionHelper.SetAttr(tool, "m_building", command.Building);

            ArrayHandler.StartApplying(command.Array16Ids, null);

            IgnoreHelper.Instance.StartIgnore();

            int mode = ReflectionHelper.GetEnumValue(typeof(TransportTool).GetNestedType("Mode", ReflectionHelper.AllAccessFlags), "AddStops");
            ReflectionHelper.SetAttr(tool, "m_mode", mode);

            IEnumerator cancelPrevStop = (IEnumerator)ReflectionHelper.Call(tool, "CancelPrevStop");
            cancelPrevStop.MoveNext();

            IgnoreHelper.Instance.EndIgnore();

            ArrayHandler.StopApplying();
        }
    }
}

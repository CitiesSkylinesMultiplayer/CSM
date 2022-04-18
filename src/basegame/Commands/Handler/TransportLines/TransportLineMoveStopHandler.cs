using System.Collections;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.TransportLines;
using CSM.BaseGame.Helpers;
using CSM.BaseGame.Injections;
using ColossalFramework;

namespace CSM.BaseGame.Commands.Handler.TransportLines
{
    public class TransportLineMoveStopHandler : CommandHandler<TransportLineMoveStopCommand>
    {
        protected override void Handle(TransportLineMoveStopCommand command)
        {
            TransportTool tool = Singleton<ToolSimulator>.instance.GetTool<TransportTool>(command.SenderId);

            ArrayHandler.StartApplying(command.Array16Ids, null);

            IgnoreHelper.Instance.StartIgnore();

            int mode = ReflectionHelper.GetEnumValue(typeof(TransportTool).GetNestedType("Mode", ReflectionHelper.AllAccessFlags), "MoveStops");
            ReflectionHelper.SetAttr(tool, "m_mode", mode);
            ReflectionHelper.SetAttr(tool, "m_errors", ToolBase.ToolErrors.None);

            IEnumerator moveStop = (IEnumerator)ReflectionHelper.Call(tool, "MoveStop", command.ApplyChanges);
            moveStop.MoveNext();

            IgnoreHelper.Instance.EndIgnore();

            ArrayHandler.StopApplying();
        }
    }
}

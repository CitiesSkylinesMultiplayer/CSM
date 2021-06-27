using CSM.Commands.Data.TransportLines;
using CSM.Helpers;
using CSM.Injections;
using System.Collections;
using CSM.API.Commands;

namespace CSM.Commands.Handler.TransportLines
{
    public class TransportLineMoveStopHandler : CommandHandler<TransportLineMoveStopCommand>
    {
        protected override void Handle(TransportLineMoveStopCommand command)
        {
            TransportTool tool = ToolSimulator.GetTool<TransportTool>(command.SenderId);

            ArrayHandler.StartApplying(command.Array16Ids, null);

            IgnoreHelper.StartIgnore();

            int mode = ReflectionHelper.GetEnumValue(typeof(TransportTool).GetNestedType("Mode", ReflectionHelper.AllAccessFlags), "MoveStops");
            ReflectionHelper.SetAttr(tool, "m_mode", mode);
            ReflectionHelper.SetAttr(tool, "m_errors", ToolBase.ToolErrors.None);

            IEnumerator moveStop = (IEnumerator)ReflectionHelper.Call(tool, "MoveStop", command.ApplyChanges);
            moveStop.MoveNext();

            IgnoreHelper.EndIgnore();

            ArrayHandler.StopApplying();
        }
    }
}

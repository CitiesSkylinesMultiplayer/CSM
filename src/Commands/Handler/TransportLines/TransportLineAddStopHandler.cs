using System.Collections;
using CSM.Commands.Data.TransportLines;
using CSM.Helpers;
using CSM.Injections;

namespace CSM.Commands.Handler.TransportLines
{
    public class TransportLineAddStopHandler : CommandHandler<TransportLineAddStopCommand>
    {
        protected override void Handle(TransportLineAddStopCommand command)
        {
            TransportTool tool = ToolSimulator.GetTool<TransportTool>(command.SenderId);

            ArrayHandler.StartApplying(command.Array16Ids, null);

            IgnoreHelper.StartIgnore();

            int mode = ReflectionHelper.GetEnumValue(typeof(TransportTool).GetNestedType("Mode", ReflectionHelper.AllAccessFlags), "AddStops");
            ReflectionHelper.SetAttr(tool, "m_mode", mode);
            ReflectionHelper.SetAttr(tool, "m_errors", ToolBase.ToolErrors.None);
            
            IEnumerator addStop = (IEnumerator) ReflectionHelper.Call(tool, "AddStop");
            addStop.MoveNext();
            
            IgnoreHelper.EndIgnore();

            ArrayHandler.StopApplying();
        }
    }
}

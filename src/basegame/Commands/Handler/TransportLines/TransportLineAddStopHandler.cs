using System.Collections;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.TransportLines;
using CSM.BaseGame.Helpers;
using CSM.BaseGame.Injections;

namespace CSM.BaseGame.Commands.Handler.TransportLines
{
    public class TransportLineAddStopHandler : CommandHandler<TransportLineAddStopCommand>
    {
        protected override void Handle(TransportLineAddStopCommand command)
        {
            TransportTool tool = ToolSimulator.GetTool<TransportTool>(command.SenderId);

            ArrayHandler.StartApplying(command.Array16Ids, null);

            IgnoreHelper.Instance.StartIgnore();

            int mode = ReflectionHelper.GetEnumValue(typeof(TransportTool).GetNestedType("Mode", ReflectionHelper.AllAccessFlags), "AddStops");
            ReflectionHelper.SetAttr(tool, "m_mode", mode);
            ReflectionHelper.SetAttr(tool, "m_errors", ToolBase.ToolErrors.None);

            IEnumerator addStop = (IEnumerator)ReflectionHelper.Call(tool, "AddStop");
            addStop.MoveNext();

            IgnoreHelper.Instance.EndIgnore();

            ArrayHandler.StopApplying();
        }
    }
}

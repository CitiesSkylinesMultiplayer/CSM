using CSM.Commands.Data.TransportLines;
using CSM.Helpers;
using CSM.Injections;

namespace CSM.Commands.Handler.TransportLines
{
    public class TransportLineMoveStopManHandler : CommandHandler<TransportLineMoveStopManCommand>
    {
        protected override void Handle(TransportLineMoveStopManCommand command)
        {
            IgnoreHelper.StartIgnore();
            ArrayHandler.StartApplying(command.Array16Ids, null);

            TransportManager.instance.m_lines.m_buffer[command.Line].MoveStop(command.Line, command.Index, command.NewPos, command.FixedPlatform);
            
            ArrayHandler.StopApplying();
            IgnoreHelper.EndIgnore();
        }
    }
}

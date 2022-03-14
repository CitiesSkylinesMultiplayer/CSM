using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.TransportLines;
using CSM.BaseGame.Injections;

namespace CSM.BaseGame.Commands.Handler.TransportLines
{
    public class TransportLineMoveStopManHandler : CommandHandler<TransportLineMoveStopManCommand>
    {
        protected override void Handle(TransportLineMoveStopManCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            ArrayHandler.StartApplying(command.Array16Ids, null);

            TransportManager.instance.m_lines.m_buffer[command.Line].MoveStop(command.Line, command.Index, command.NewPos, command.FixedPlatform);

            ArrayHandler.StopApplying();
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

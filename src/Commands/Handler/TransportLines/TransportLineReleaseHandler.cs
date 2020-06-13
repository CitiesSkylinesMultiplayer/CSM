using CSM.Commands.Data.TransportLines;
using CSM.Helpers;
using CSM.Injections;

namespace CSM.Commands.Handler.TransportLines
{
    public class TransportLineReleaseHandler : CommandHandler<TransportLineReleaseCommand>
    {
        protected override void Handle(TransportLineReleaseCommand command)
        {
            IgnoreHelper.StartIgnore();
            ArrayHandler.StartApplying(command.Array16Ids, null);

            TransportManager.instance.ReleaseLine(command.Line);

            ArrayHandler.StopApplying();
            IgnoreHelper.EndIgnore();
        }
    }
}

using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.TransportLines;
using CSM.BaseGame.Injections;

namespace CSM.BaseGame.Commands.Handler.TransportLines
{
    public class TransportLineReleaseHandler : CommandHandler<TransportLineReleaseCommand>
    {
        protected override void Handle(TransportLineReleaseCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            ArrayHandler.StartApplying(command.Array16Ids, null);

            TransportManager.instance.ReleaseLine(command.Line);

            ArrayHandler.StopApplying();
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

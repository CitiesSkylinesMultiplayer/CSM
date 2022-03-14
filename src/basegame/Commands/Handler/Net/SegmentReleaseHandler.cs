using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Net;

namespace CSM.BaseGame.Commands.Handler.Net
{
    public class SegmentReleaseHandler : CommandHandler<SegmentReleaseCommand>
    {
        protected override void Handle(SegmentReleaseCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            NetManager.instance.ReleaseSegment(command.SegmentId, command.KeepNodes);
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}

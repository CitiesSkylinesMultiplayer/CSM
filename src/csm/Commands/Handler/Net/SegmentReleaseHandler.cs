using CSM.API.Commands;
using CSM.Commands.Data.Net;
using CSM.Helpers;

namespace CSM.Commands.Handler.Net
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

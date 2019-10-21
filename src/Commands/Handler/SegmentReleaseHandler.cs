using ColossalFramework;
using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class SegmentReleaseHandler : CommandHandler<SegmentReleaseCommand>
    {
        public override void Handle(SegmentReleaseCommand command)
        {
            NetHandler.IgnoreAll = true;
            NetManager.instance.ReleaseSegment(command.SegmentId, command.KeepNodes);
            NetHandler.IgnoreAll = false;
        }
    }
}

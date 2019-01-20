using ColossalFramework;
using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class SegmentReleaseHandler : CommandHandler<SegmentReleaseCommand>
    {
        public override void Handle(SegmentReleaseCommand command)
        {
            NodeHandler.IgnoreSegments.Add(command.SegmentId);
            Singleton<NetManager>.instance.ReleaseSegment(command.SegmentId, command.KeepNodes);
            NodeHandler.IgnoreSegments.Remove(command.SegmentId);
        }
    }
}

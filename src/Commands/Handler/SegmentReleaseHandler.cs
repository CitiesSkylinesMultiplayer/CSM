using ColossalFramework;
using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class SegmentReleaseHandler : CommandHandler<SegmentReleaseCommand>
    {
        public override void Handle(SegmentReleaseCommand command)
        {
            NodeHandler.IgnoreAll = true;
            Singleton<NetManager>.instance.ReleaseSegment(command.SegmentId, command.KeepNodes);
            NodeHandler.IgnoreAll = false;
        }
    }
}

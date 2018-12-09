using ColossalFramework;
using CSM.Injections;
using CSM.Networking;
using System.Linq;

namespace CSM.Commands.Handler
{
    class SegmentReleaseHandler : CommandHandler<SegmentReleaseCommand>
    {
        public override byte ID => 113;

        public override void HandleOnServer(SegmentReleaseCommand command, Player player) => Handle(command);

        public override void HandleOnClient(SegmentReleaseCommand command) => Handle(command);

        private void Handle(SegmentReleaseCommand command)
        {
            NodeHandler.IgnoreSegments.Add(command.SegmentId);
            Singleton<NetManager>.instance.ReleaseSegment(command.SegmentId, command.KeepNodes);
            NodeHandler.IgnoreSegments.Remove(command.SegmentId);
        }
    }
}

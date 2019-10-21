
namespace CSM.Commands.Handler
{
    public class SegmentSeedUpdateHandler : CommandHandler<SegmentSeedUpdateCommand>
    {
        public override void Handle(SegmentSeedUpdateCommand command)
        {
            NetManager.instance.m_segments.m_buffer[command.SegmentId].m_nameSeed = command.NameSeed;
            // Update name (+ make sure that we don't overwrite custom set names)
            NetManager.instance.SetSegmentNameImpl(command.SegmentId, NetManager.instance.GetSegmentName(command.SegmentId));
        }
    }
}

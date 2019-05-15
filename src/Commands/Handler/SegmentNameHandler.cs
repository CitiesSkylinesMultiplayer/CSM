using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class SegmentNameHandler : CommandHandler<SegmentNameCommand>
    {
        public override void Handle(SegmentNameCommand command)
        {
            NodeHandler.IgnoreAll = true;
            while (NetManager.instance.SetSegmentName(command.SegmentID, command.Name).MoveNext()) { break; }
            NodeHandler.IgnoreAll = false;
        }
    }
}

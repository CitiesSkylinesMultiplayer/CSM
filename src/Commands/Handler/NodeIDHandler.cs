using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class NodeIdHandler : CommandHandler<NodeIdCommand>
    {
        public override byte ID => 113;

        public override void HandleOnServer(NodeIdCommand command, Player player) => HandleNodeID(command);

        public override void HandleOnClient(NodeIdCommand command) => HandleNodeID(command);

        private void HandleNodeID(NodeIdCommand command)
        {
            Extensions.NodeAndSegmentExtension.NodeIDDictionary.Add((ushort)command.NodeIdSender, (ushort)command.NodeIdReciever);
        }
    }
}
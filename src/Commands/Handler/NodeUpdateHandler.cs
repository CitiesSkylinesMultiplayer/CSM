using CSM.Networking;
using ColossalFramework;

namespace CSM.Commands.Handler
{
    public class NodeUpdateHandler : CommandHandler<NodeUpdateCommand>
    {
        public override byte ID => 111;

        public override void HandleOnServer(NodeUpdateCommand command, Player player) => Handle(command);

        public override void HandleOnClient(NodeUpdateCommand command) => Handle(command);

        private void Handle(NodeUpdateCommand command)
        {
            var nodeid = command.NodeID;
            Singleton<NetManager>.instance.m_nodes.m_buffer[nodeid].m_segment0 = command.Segment0;
            Singleton<NetManager>.instance.m_nodes.m_buffer[nodeid].m_segment1 = command.Segment1;
            Singleton<NetManager>.instance.m_nodes.m_buffer[nodeid].m_segment2 = command.Segment2;
            Singleton<NetManager>.instance.m_nodes.m_buffer[nodeid].m_segment3 = command.Segment3;
            Singleton<NetManager>.instance.m_nodes.m_buffer[nodeid].m_segment4 = command.Segment4;
            Singleton<NetManager>.instance.m_nodes.m_buffer[nodeid].m_segment5 = command.Segment5;
            Singleton<NetManager>.instance.m_nodes.m_buffer[nodeid].m_segment6 = command.Segment6;
            Singleton<NetManager>.instance.m_nodes.m_buffer[nodeid].m_segment7 = command.Segment7;
        }
    }

}

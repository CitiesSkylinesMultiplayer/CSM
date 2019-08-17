using ColossalFramework;

namespace CSM.Commands.Handler
{
    public class NodeUpdateHandler : CommandHandler<NodeUpdateCommand>
    {
        public override void Handle(NodeUpdateCommand command)
        {
            ushort nodeId = command.NodeId;
            Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].m_segment0 = command.Segments[0];
            Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].m_segment1 = command.Segments[1];
            Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].m_segment2 = command.Segments[2];
            Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].m_segment3 = command.Segments[3];
            Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].m_segment4 = command.Segments[4];
            Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].m_segment5 = command.Segments[5];
            Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].m_segment6 = command.Segments[6];
            Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].m_segment7 = command.Segments[7];
            Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].m_flags = command.Flags;
        }
    }
}

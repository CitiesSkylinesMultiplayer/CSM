using ProtoBuf;
using UnityEngine;

namespace CSM.Models
{
    [ProtoContract]
    public class ControlPointSurrogate
    {
        [ProtoMember(1)]
        public Vector3 position { get; set; }

        [ProtoMember(2)]
        public Vector3 direction { get; set; }

        [ProtoMember(3)]
        public ushort node { get; set; }
        
        [ProtoMember(4)]
        public ushort segment { get; set; }
        
        [ProtoMember(5)]
        public float elevation { get; set; }
        
        [ProtoMember(6)]
        public bool outside { get; set; }

        public static implicit operator ControlPointSurrogate(NetTool.ControlPoint value)
        {
            return new ControlPointSurrogate
            {
                position = value.m_position,
                direction = value.m_direction,
                node = value.m_node,
                segment = value.m_segment,
                elevation = value.m_elevation,
                outside = value.m_outside
            };
        }

        public static implicit operator NetTool.ControlPoint(ControlPointSurrogate value)
        {
            return new NetTool.ControlPoint
            {
                m_position = value.position,
                m_direction = value.direction,
                m_node = value.node,
                m_segment = value.segment,
                m_elevation = value.elevation,
                m_outside = value.outside
            };
        }
    }
}

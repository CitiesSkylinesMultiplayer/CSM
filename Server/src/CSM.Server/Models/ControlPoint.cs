using ProtoBuf;

namespace CSM.Models
{
    [ProtoContract]
    public class ControlPoint
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
    }
}

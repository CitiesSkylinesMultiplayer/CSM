using ProtoBuf;

namespace CSM.Models
{
    [ProtoContract]
    public class Vector3
    {
        [ProtoMember(1)]
        public float X { get; set; }

        [ProtoMember(2)]
        public float Y { get; set; }

        [ProtoMember(3)]
        public float Z { get; set; }
    }
}

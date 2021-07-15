using ProtoBuf;

namespace CSM.Models
{
    [ProtoContract]
    public class Color
    {
        [ProtoMember(1)]
        public float R { get; set; }

        [ProtoMember(2)]
        public float G { get; set; }

        [ProtoMember(3)]
        public float B { get; set; }

        [ProtoMember(4)]
        public float A { get; set; }
    }
}

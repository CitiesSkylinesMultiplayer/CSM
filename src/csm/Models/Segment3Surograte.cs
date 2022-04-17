using ProtoBuf;
using UnityEngine;
using ColossalFramework.Math;

namespace CSM.Models
{
    [ProtoContract]
    public class Segment3Surrogate
    {
        [ProtoMember(1)]
        public Vector3 a { get; set; }
        [ProtoMember(2)]
        public Vector3 b { get; set; }

        public static implicit operator Segment3Surrogate(Segment3 value)
        {
            return new Segment3Surrogate
            {
                a = value.a,
                b = value.b
            };
        }

        public static implicit operator Segment3(Segment3Surrogate value)
        {
            return new Segment3(value.a, value.b);
        }
    }
}

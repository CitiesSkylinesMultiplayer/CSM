using ProtoBuf;
using UnityEngine;

namespace CSM.Models
{
    [ProtoContract]
    public class QuaternionSurrogate
    {
        [ProtoMember(1)]
        public float X { get; set; }

        [ProtoMember(2)]
        public float Y { get; set; }

        [ProtoMember(3)]
        public float Z { get; set; }

        [ProtoMember(4)]
        public float W { get; set; }

        public static implicit operator QuaternionSurrogate(Quaternion value)
        {
            return new QuaternionSurrogate
            {
                X = value.x,
                Y = value.y,
                Z = value.z,
                W = value.w
            };
        }

        public static implicit operator Quaternion(QuaternionSurrogate value)
        {
            return new Quaternion { x = value.X, y = value.Y, z = value.Z, w = value.W };
        }
    }
}

using ProtoBuf;
using UnityEngine;

namespace CSM.Models
{
    [ProtoContract]
    public class Vector3Surrogate
    {
        [ProtoMember(1)]
        public float X { get; set; }

        [ProtoMember(2)]
        public float Y { get; set; }

        [ProtoMember(3)]
        public float Z { get; set; }

        public static implicit operator Vector3Surrogate(Vector3 value)
        {
            return new Vector3Surrogate
            {
                X = value.x,
                Y = value.y,
                Z = value.z
            };
        }

        public static implicit operator Vector3(Vector3Surrogate value)
        {
            return new Vector3 { x = value.X, y = value.Y, z = value.Z };
        }
    }
}
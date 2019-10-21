using ProtoBuf;
using UnityEngine;

namespace CSM.Models
{
    [ProtoContract]
    public class ColorSurrogate
    {
        [ProtoMember(1)]
        public float R { get; set; }

        [ProtoMember(2)]
        public float G { get; set; }

        [ProtoMember(3)]
        public float B { get; set; }

        [ProtoMember(4)]
        public float A { get; set; }

        public static implicit operator ColorSurrogate(Color value)
        {
            return new ColorSurrogate
            {
                R = value.r,
                G = value.g,
                B = value.b,
                A = value.a
            };
        }

        public static implicit operator Color(ColorSurrogate value)
        {
            return new Color { r = value.R, g = value.G, b = value.B, a = value.A };
        }
    }
}

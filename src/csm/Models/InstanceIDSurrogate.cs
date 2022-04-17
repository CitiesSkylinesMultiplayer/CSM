using ProtoBuf;
using UnityEngine;

namespace CSM.Models
{
    [ProtoContract]
    public class InstanceIDSurrogate
    {
        [ProtoMember(1)]
        public uint id { get; set; }

        public static implicit operator InstanceIDSurrogate(InstanceID value)
        {
            return new InstanceIDSurrogate
            {
                id = value.RawData
            };
        }

        public static implicit operator InstanceID(InstanceIDSurrogate value)
        {
            return new InstanceID { RawData = value.id };
        }
    }
}

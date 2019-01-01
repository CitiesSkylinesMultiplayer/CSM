using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class DistrictCreateCommand : CommandBase
    {
        [ProtoMember(1)]
        public byte DistrictID { get; set; }
    }
}

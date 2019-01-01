using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class DistrictReleaseCommand : CommandBase
    {
        [ProtoMember(1)]
        public byte DistrictID { get; set; }
    }
}

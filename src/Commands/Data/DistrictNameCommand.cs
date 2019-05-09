using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class DistrictNameCommand : CommandBase
    {
        [ProtoMember(1)]
        public byte DistrictID { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }
    }
}

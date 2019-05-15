using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class SegmentNameCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort SegmentID { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }
    }
}

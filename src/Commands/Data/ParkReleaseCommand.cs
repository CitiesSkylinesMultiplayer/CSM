using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    public class ParkReleaseCommand : CommandBase
    {
        [ProtoMember(1)]
        public byte ParkID { get; set; }
    }
}

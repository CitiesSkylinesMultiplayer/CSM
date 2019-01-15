using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class PropReleaseCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort PropID { get; set; }
    }
}

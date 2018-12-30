using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class TreeReleaseCommand : CommandBase
    {
        [ProtoMember(1)]
        public uint TreeID { get; set; }
    }
}

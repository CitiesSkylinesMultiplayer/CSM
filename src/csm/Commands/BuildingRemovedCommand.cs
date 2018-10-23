using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class BuildingRemovedCommand : CommandBase
    {
        [ProtoMember(1)]
        public Vector3 Position { get; set; }
    }
}
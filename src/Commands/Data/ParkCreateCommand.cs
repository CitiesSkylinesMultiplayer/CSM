using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class ParkCreateCommand : CommandBase
    {
        [ProtoMember(1)]
        public byte ParkID { get; set; }

        [ProtoMember(2)]
        public DistrictPark.ParkType ParkType { get; set; }

        [ProtoMember(3)]
        public DistrictPark.ParkLevel ParkLevel { get; set; }
        
        [ProtoMember(4)]
        public ulong Seed { get; set; }
    }
}

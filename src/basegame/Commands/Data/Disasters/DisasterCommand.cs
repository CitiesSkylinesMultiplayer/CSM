using CSM.API.Commands;
using ProtoBuf;
using UnityEngine;

namespace CSM.BaseGame.Commands.Data.Disasters
{
    [ProtoContract]
    public class DisasterCommand : CommandBase
    {
        [ProtoMember(1)]
        public uint InfoIndex { get; set; }

        [ProtoMember(2)]
        public Vector3 Position { get; set; }

        [ProtoMember(3)]
        public int Intensity { get; set; }

        [ProtoMember(4)]
        public float Angle { get; set; }

        [ProtoMember(5)]
        public ulong RandomSeed { get; set; }
    }
}

using ProtoBuf;
using ICities;

namespace CSM.Commands.Data.Disasters
{
    /// <summary>
    ///     Sent when a natural disaster is created
    /// </summary>
    /// Sent by:
    /// - DisasterExtension
    [ProtoContract]
    public class DisasterCreateCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort Id { get; set; }

        [ProtoMember(2)]
        public DisasterType Type { get; set; }

        [ProtoMember(3)]
        public string Name { get; set; }

        [ProtoMember(4)]
        public float TargetX { get; set; }

        [ProtoMember(5)]
        public float TargetY { get; set; }

        [ProtoMember(6)]
        public float TargetZ { get; set; }

        [ProtoMember(7)]
        public float Angle { get; set; }

        [ProtoMember(8)]
        public byte Intensity { get; set; }

        /// <summary>
        ///     The client id of the disconnected user (to clear caches).
        /// </summary>
        [ProtoMember(9)]
        public int ClientId { get; set; }
    }
}
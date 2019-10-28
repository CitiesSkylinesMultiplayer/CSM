using ProtoBuf;

namespace CSM.Commands.Data.TransportLines
{
    /// <summary>
    ///     Called when a line was released.
    /// </summary>
    /// Sent by:
    /// - TransportHandler
    [ProtoContract]
    public class TransportLineReleaseCommand : CommandBase
    {
        /// <summary>
        ///     The released line.
        /// </summary>
        [ProtoMember(1)]
        public ushort Line { get; set; }

        /// <summary>
        ///     The list of generated Array16 ids.
        /// </summary>
        [ProtoMember(2)]
        public ushort[] Array16Ids { get; set; }
    }
}

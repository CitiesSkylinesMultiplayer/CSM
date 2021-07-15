using ProtoBuf;

namespace CSM.Commands.Data.TransportLines
{
    /// <summary>
    ///     Called when a stop is moved.
    /// </summary>
    /// Sent by:
    /// - TransportHandler
    [ProtoContract]
    public class TransportLineMoveStopCommand : CommandBase
    {
        /// <summary>
        ///     The list of generated Array16 ids.
        /// </summary>
        [ProtoMember(1)]
        public ushort[] Array16Ids { get; set; }

        /// <summary>
        ///     If the ApplyChanges flag is set.
        /// </summary>
        [ProtoMember(2)]
        public bool ApplyChanges { get; set; }
    }
}

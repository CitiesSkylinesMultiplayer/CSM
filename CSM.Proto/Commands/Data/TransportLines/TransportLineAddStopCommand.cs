using ProtoBuf;

namespace CSM.Commands.Data.TransportLines
{
    /// <summary>
    ///     Called when a stop is added to the transport line.
    /// </summary>
    /// Sent by:
    /// - TransportHandler
    [ProtoContract]
    public class TransportLineAddStopCommand : CommandBase
    {
        /// <summary>
        ///     The list of generated Array16 ids.
        /// </summary>
        [ProtoMember(1)]
        public ushort[] Array16Ids { get; set; }
    }
}

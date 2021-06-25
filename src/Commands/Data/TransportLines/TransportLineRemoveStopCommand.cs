using CSM.API.Commands;
using ProtoBuf;

namespace CSM.Commands.Data.TransportLines
{
    /// <summary>
    ///     Called when a stop is removed from a line.
    /// </summary>
    /// Sent by:
    /// - TransportHandler
    [ProtoContract]
    public class TransportLineRemoveStopCommand : CommandBase
    {
        /// <summary>
        ///     The list of generated Array16 ids.
        /// </summary>
        [ProtoMember(1)]
        public ushort[] Array16Ids { get; set; }

        /// <summary>
        ///     The currently activated building.
        /// </summary>
        [ProtoMember(2)]
        public int Building { get; set; }

        /// <summary>
        ///     The currently activated prefab info index.
        /// </summary>
        [ProtoMember(3)]
        public ushort Prefab { get; set; }
    }
}

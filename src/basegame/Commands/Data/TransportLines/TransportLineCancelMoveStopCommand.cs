using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.TransportLines
{
    /// <summary>
    ///     Called when a stop move was cancelled.
    ///     Used to keep the tool class synced.
    /// </summary>
    /// Sent by:
    /// - TransportHandler
    [ProtoContract]
    public class TransportLineCancelMoveStopCommand : CommandBase
    {
        /// <summary>
        ///     The list of generated Array16 ids.
        /// </summary>
        [ProtoMember(1)]
        public ushort[] Array16Ids { get; set; }

        /// <summary>
        ///     The currently activated prefab info index.
        /// </summary>
        [ProtoMember(3)]
        public ushort Prefab { get; set; }
    }
}

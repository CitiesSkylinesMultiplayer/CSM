using CSM.API.Commands;
using ProtoBuf;

namespace CSM.Commands.Data.Internal
{
    /// <summary>
    ///     This commands transfers the save game.
    /// </summary>
    /// Sent by:
    /// - ConnectionRequestHandler
    [ProtoContract]
    public class WorldTransferCommand : CommandBase
    {
        /// <summary>
        ///     A slice of the serialized save game.
        /// </summary>
        [ProtoMember(1)]
        public byte[] WorldSlice { get; set; }
        
        /// <summary>
        ///     The remaining bytes of the save game to transmit.
        /// </summary>
        [ProtoMember(2)]
        public int RemainingBytes { get; set; }
        
        /// <summary>
        ///     Is true, when a new world transfer is started.
        /// </summary>
        [ProtoMember(3)]
        public bool NewTransfer { get; set; }
    }
}

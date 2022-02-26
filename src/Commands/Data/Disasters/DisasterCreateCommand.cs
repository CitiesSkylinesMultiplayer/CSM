using ProtoBuf;

namespace CSM.Commands.Data.Disasters
{
    /// <summary>
    ///     Sent when a loan has been paid back.
    /// </summary>
    /// Sent by:
    /// - EconomyHandler
    [ProtoContract]
    public class DisasterStartCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort Id { get; set; }

        /// <summary>
        ///     The client id of the disconnected user (to clear caches).
        /// </summary>
        [ProtoMember(2)]
        public int ClientId { get; set; }
    }
}
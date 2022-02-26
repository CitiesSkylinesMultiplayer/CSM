using ProtoBuf;

namespace CSM.Commands.Data.Disasters
{
    /// <summary>
    ///     Sent when a natural disaster is started
    /// </summary>
    /// Sent by:
    /// - DisasterHelper
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
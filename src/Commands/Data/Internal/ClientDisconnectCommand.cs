using ProtoBuf;

namespace CSM.Commands.Data.Internal
{
    /// <summary>
    ///     The server sends this command to all connected clients when
    ///     another client disconnects.
    /// </summary>
    /// Sent by:
    /// - ClientDisconnectHandler
    [ProtoContract]
    public class ClientDisconnectCommand : CommandBase
    {
        /// <summary>
        ///     The user name of the disconnected user.
        /// </summary>
        [ProtoMember(1)]
        public string Username { get; set; }

        /// <summary>
        ///     The client id of the disconnected user (to clear caches).
        /// </summary>
        [ProtoMember(2)]
        public int ClientId { get; set; }
    }
}

using ProtoBuf;

namespace CSM.Commands
{
    /// <summary>
    ///     The server sends this command to all connected clients when
    ///     another client disconnects.
    /// </summary>
    [ProtoContract]
    public class ClientDisconnectCommand : CommandBase
    {
        /// <summary>
        ///     The user name of the disconnected user.
        /// </summary>
        [ProtoMember(1)]
        public string Username { get; set; }

        /// <summary>
        ///     The client id of the disconnected user.
        /// </summary>
        [ProtoMember(2)]
        public int ClientId { get; set; }
    }
}
using ProtoBuf;

namespace CSM.Commands.Data.Internal
{
    /// <summary>
    ///     The server sends this command to all connected clients when
    ///     another client connects to the game.
    /// </summary>
    /// Sent by:
    /// - ClientConnectHandler
    [ProtoContract]
    [FixedCommand(90002)]
    public class ClientConnectCommand : CommandBase
    {
        /// <summary>
        ///     The user name of the newly connected user.
        /// </summary>
        [ProtoMember(1)]
        public string Username { get; set; }
    }
}

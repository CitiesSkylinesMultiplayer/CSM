using ProtoBuf;

namespace CSM.Commands
{
    /// <summary>
    ///     The server sends this command to all connected clients when
    ///     another client connects to the game.
    /// </summary>
    [ProtoContract]
    public class ClientConnectCommand : CommandBase
    {
        /// <summary>
        ///     The user name of the disconnected user.
        /// </summary>
        [ProtoMember(1)]
        public string Username { get; set; }
    }
}
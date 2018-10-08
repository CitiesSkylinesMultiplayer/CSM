using ProtoBuf;
using System.IO;

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

        /// <summary>
        ///     Deserialize a message into this type.
        /// </summary>
        public static ClientConnectCommand Deserialize(byte[] message)
        {
            ClientConnectCommand result;

            using (var stream = new MemoryStream(message))
            {
                result = Serializer.Deserialize<ClientConnectCommand>(stream);
            }

            return result;
        }
    }
}
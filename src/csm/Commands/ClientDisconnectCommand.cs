using ProtoBuf;
using System.IO;

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
        ///     The user name of the newly connected user
        /// </summary>
        [ProtoMember(1)]
        public string Username { get; set; }

        /// <summary>
        ///     Deserialize a message into this type.
        /// </summary>
        public static ClientDisconnectCommand Deserialize(byte[] message)
        {
            ClientDisconnectCommand result;

            using (var stream = new MemoryStream(message))
            {
                result = Serializer.Deserialize<ClientDisconnectCommand>(stream);
            }

            return result;
        }
    }
}
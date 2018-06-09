using System.IO;
using ProtoBuf;

namespace CitiesSkylinesMultiplayer.Commands
{
    /// <summary>
    ///     The server sends this command to all connected clients when
    ///     another client connects to the game.
    /// </summary>
    [ProtoContract]
    public class ClientConnect : CommandBase
    {
        /// <summary>
        ///     The username of the disconnected user.
        /// </summary>
        [ProtoMember(1)]
        public string Username { get; set; }

        /// <summary>
        ///     Deserialize a message into this type.
        /// </summary>
        public static ConnectionResult Deserialize(byte[] message)
        {
            ConnectionResult result;

            using (var stream = new MemoryStream(message))
            {
                result = Serializer.Deserialize<ConnectionResult>(stream);
            }

            return result;
        }
    }
}

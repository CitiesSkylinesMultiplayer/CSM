using System.IO;
using ProtoBuf;

namespace CitiesSkylinesMultiplayer.Commands
{
    /// <summary>
    ///     The first message send to a server, asks the server if
    ///     the current client can connect. The server will then validate this
    ///     information, store it, and then send back a connection result  
    /// </summary>
    [ProtoContract]
    public class ConnectionRequest : CommandBase
    {
        /// <summary>
        ///     The username this user will be playing as, important
        ///     as the server will keep track of this user.
        /// </summary>
        [ProtoMember(1)]
        public string Username { get; set; }

        /// <summary>
        ///     An optional password if the server is setup to
        ///     require a password.
        /// </summary>
        [ProtoMember(2)]
        public string Password { get; set; }

        /// <summary>
        ///     How many mods the client has installed. A simple and dirty way
        ///     to perform checking to see if mods match.
        /// </summary>
        [ProtoMember(3)]
        public int ModCount { get; set; }

        /// <summary>
        ///     What version of the mod this user has installed. The server needs to check
        ///     that both version match (for obvious reasons, as major updates may of happened).
        /// </summary>
        [ProtoMember(4)]
        public string ModVersion { get; set; }

        /// <summary>
        ///     What version of the game is the user running. There might be issues between games,
        ///     so we need to check that these match.
        /// </summary>
        [ProtoMember(5)]
        public string GameVersion { get; set; }

        /// <summary>
        ///     Deserialize a message into this type.
        /// </summary>
        public static ConnectionRequest Deserialize(byte[] message)
        {
            ConnectionRequest result;

            using (var stream = new MemoryStream(message))
            {
                result = Serializer.Deserialize<ConnectionRequest>(stream);
            }

            return result;
        }
    }
}
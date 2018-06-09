using ProtoBuf;

namespace CitiesSkylinesMultiplayer.Commands
{
    /// <summary>
    ///     The server sends this command to all connected clients when
    ///     another client disconnects. 
    /// </summary>
    [ProtoContract]
    public class ClientDisconnect
    {
        /// <summary>
        ///     The username of the newly connected user
        /// </summary>
        [ProtoMember(1)]
        public string Username { get; set; }
    }
}
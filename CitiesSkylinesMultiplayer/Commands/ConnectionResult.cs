using ProtoBuf;

namespace CitiesSkylinesMultiplayer.Commands
{
    /// <summary>
    ///     Returned from the server after the server recieves
    ///     as connection request command. Contains if the server
    ///     accepts the connection or not (wrong password, incorrect mods,
    ///     different game version etc.)
    /// </summary>
    [ProtoContract]
    public class ConnectionResult
    {
        /// <summary>
        ///     If the server accepts the connection
        /// </summary>
        [ProtoMember(1)]
        public bool Success { get; set; }

        /// <summary>
        ///     If success is false, this will contain a reason why
        ///     the serve rejected the request.
        /// </summary>
        [ProtoMember(2)]
        public string Reason { get; set; }
    }
}
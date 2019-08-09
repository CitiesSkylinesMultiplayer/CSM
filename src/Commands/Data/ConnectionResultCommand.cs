using ProtoBuf;

namespace CSM.Commands
{
    /// <summary>
    ///     Returned from the server after the server receives
    ///     as connection request command. Contains if the server
    ///     accepts the connection or not (wrong password, incorrect mods,
    ///     different game version etc.)
    /// </summary>
    [ProtoContract]
    public class ConnectionResultCommand : CommandBase
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

        /// <summary>
        ///     The assigned client id if the connection
        ///     request was successful.
        /// </summary>
        [ProtoMember(3)]
        public int ClientId { get; set; }

        /// <summary>
        ///     The world in a serialized byte array.
        /// </summary>
        [ProtoMember(4)]
        public byte[] World { get; set; }

        /// <summary>
        ///     Contains the DLCs of the server for comparison, when DLCs of the client and server don't match
        /// </summary>
        [ProtoMember(5)]
        public SteamHelper.DLC_BitMask DLCBitMask { get; set; }
    }
}
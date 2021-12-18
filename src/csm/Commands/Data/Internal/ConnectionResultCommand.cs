using CSM.API.Commands;
using ProtoBuf;

namespace CSM.Commands.Data.Internal
{
    /// <summary>
    ///     Returned from the server after the server receives
    ///     the connection request command. Contains if the server
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
        ///     the server rejected the request.
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
        ///     Contains the DLC bit mask of the server for comparison,
        ///     when DLCs of the client and server don't match.
        /// </summary>
        [ProtoMember(4)]
        public SteamHelper.DLC_BitMask DLCBitMask { get; set; }
    }
}

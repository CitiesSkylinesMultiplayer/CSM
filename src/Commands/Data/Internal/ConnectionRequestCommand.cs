using ProtoBuf;

namespace CSM.Commands.Data.Internal
{
    /// <summary>
    ///     The first message sent to a server, asks the server if
    ///     the current client can connect. The server will then validate this
    ///     information, store it, and then send back a connection result.
    /// </summary>
    /// Sent by:
    /// - Client
    [ProtoContract]
    public class ConnectionRequestCommand : CommandBase
    {
        /// <summary>
        ///     The user name this user will be playing as, important
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
        ///     that both version match (for obvious reasons, as major updates may have happened).
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
        ///     If true client should wait for a WorldTransferCommand
        /// </summary>
        [ProtoMember(6)]
        public bool RequestWorld { get; set; }

        /// <summary>
        ///     BitMask containing the installed DLCs of the client
        /// </summary>
        [ProtoMember(7)]
        public SteamHelper.DLC_BitMask DLCBitMask { get; set; }
    }
}

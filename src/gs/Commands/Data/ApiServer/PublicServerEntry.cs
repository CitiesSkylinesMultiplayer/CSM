using ProtoBuf;

namespace CSM.GS.Commands.Data.ApiServer
{
    /// <summary>
    ///     A single entry in the public server list. Only contains information
    ///     safe to expose publicly (no internal/external IP distinction - joining
    ///     goes through the NAT-relay token, same as a Steam friend-invite join).
    /// </summary>
    [ProtoContract]
    public class PublicServerEntry
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public int CurrentPlayers { get; set; }

        [ProtoMember(3)]
        public int MaxPlayers { get; set; }

        [ProtoMember(4)]
        public bool HasPassword { get; set; }

        /// <summary>
        ///     The server's NAT-relay token, used to join via JoinGamePanel.JoinByToken -
        ///     the same mechanism used for Steam friend-invite joins.
        /// </summary>
        [ProtoMember(6)]
        public string ServerToken { get; set; }
    }
}

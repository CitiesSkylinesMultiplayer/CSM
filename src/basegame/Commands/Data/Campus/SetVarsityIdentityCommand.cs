using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Campus
{
    /// <summary>
    ///     Called when the identity of the varsity sports is set for a campus.
    /// </summary>
    /// Sent by:
    /// - CampusHandler
    [ProtoContract]
    public class SetVarsityIdentityCommand : CommandBase
    {
        /// <summary>
        ///     The park id of the campus.
        /// </summary>
        [ProtoMember(1)]
        public byte Campus { get; set; }

        /// <summary>
        ///     The new varsity identity.
        /// </summary>
        [ProtoMember(2)]
        public int Identity { get; set; }
    }
}

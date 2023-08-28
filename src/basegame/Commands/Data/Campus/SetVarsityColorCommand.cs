using CSM.API.Commands;
using ProtoBuf;
using UnityEngine;

namespace CSM.BaseGame.Commands.Data.Campus
{
    /// <summary>
    ///     Called when the color of the varsity sports is set for a campus.
    /// </summary>
    /// Sent by:
    /// - CampusHandler
    [ProtoContract]
    public class SetVarsityColorCommand : CommandBase
    {
        /// <summary>
        ///     The park id of the campus.
        /// </summary>
        [ProtoMember(1)]
        public byte Campus { get; set; }

        /// <summary>
        ///     The new varsity color.
        /// </summary>
        [ProtoMember(2)]
        public Color Color { get; set; }
    }
}

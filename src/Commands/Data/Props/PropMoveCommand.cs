using ProtoBuf;
using UnityEngine;

namespace CSM.Commands.Data.Props
{
    /// <summary>
    ///     Called when a prop is moved.
    /// </summary>
    /// Sent by:
    /// - PropHandler
    [ProtoContract]
    public class PropMoveCommand : CommandBase
    {
        /// <summary>
        ///     The id of the prop to move.
        /// </summary>
        [ProtoMember(1)]
        public ushort PropId { get; set; }

        /// <summary>
        ///     The new position of the prop.
        /// </summary>
        [ProtoMember(2)]
        public Vector3 Position { get; set; }
    }
}

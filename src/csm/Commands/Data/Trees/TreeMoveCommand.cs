using CSM.API.Commands;
using ProtoBuf;
using UnityEngine;

namespace CSM.Commands.Data.Trees
{
    /// <summary>
    ///     Called when a tree is moved.
    /// </summary>
    /// Sent by:
    /// - TreeHandler
    [ProtoContract]
    public class TreeMoveCommand : CommandBase
    {
        /// <summary>
        ///     The id of the tree to move.
        /// </summary>
        [ProtoMember(1)]
        public uint TreeId { get; set; }

        /// <summary>
        ///     The new position of the tree.
        /// </summary>
        [ProtoMember(2)]
        public Vector3 Position { get; set; }
    }
}

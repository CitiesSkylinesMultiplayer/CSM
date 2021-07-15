using ProtoBuf;
using UnityEngine;

namespace CSM.Commands.Data.Trees
{
    /// <summary>
    ///     Called when a tree is created.
    /// </summary>
    /// Sent by:
    /// - TreeHandler
    [ProtoContract]
    public class TreeCreateCommand : CommandBase
    {
        /// <summary>
        ///     The id of the new tree.
        /// </summary>
        [ProtoMember(1)]
        public uint TreeId { get; set; }

        /// <summary>
        ///     The position to create the tree on.
        /// </summary>
        [ProtoMember(2)]
        public Vector3 Position { get; set; }

        /// <summary>
        ///     If a single tree was created.
        /// </summary>
        [ProtoMember(3)]
        public bool Single { get; set; }

        /// <summary>
        ///     The info index of the prefab to create.
        /// </summary>
        [ProtoMember(4)]
        public ushort InfoIndex { get; set; }
    }
}

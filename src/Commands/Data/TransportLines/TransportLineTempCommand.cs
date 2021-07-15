using ProtoBuf;
using UnityEngine;

namespace CSM.Commands.Data.TransportLines
{
    /// <summary>
    ///     Called when the temp line is updated.
    /// </summary>
    /// Sent by:
    /// - TransportHandler
    [ProtoContract]
    public class TransportLineTempCommand : CommandBase
    {
        /// <summary>
        ///     The new prefab info index.
        /// </summary>
        [ProtoMember(1)]
        public ushort InfoIndex { get; set; }

        /// <summary>
        ///     The source line this temp line refers to.
        /// </summary>
        [ProtoMember(2)]
        public ushort SourceLine { get; set; }

        /// <summary>
        ///     The index of the stop that will be moved.
        /// </summary>
        [ProtoMember(3)]
        public int MoveIndex { get; set; }

        /// <summary>
        ///     The index of the stop that will be added.
        /// </summary>
        [ProtoMember(4)]
        public int AddIndex { get; set; }

        /// <summary>
        ///     The position of a new stop.
        /// </summary>
        [ProtoMember(5)]
        public Vector3 AddPos { get; set; }

        /// <summary>
        ///     If the line has fixed platforms.
        /// </summary>
        [ProtoMember(6)]
        public bool FixedPlatform { get; set; }

        /// <summary>
        ///     The list of generated Array16 ids.
        /// </summary>
        [ProtoMember(7)]
        public ushort[] Array16Ids { get; set; }
    }
}

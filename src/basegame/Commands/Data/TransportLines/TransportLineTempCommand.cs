using CSM.API.Commands;
using ProtoBuf;
using UnityEngine;

namespace CSM.BaseGame.Commands.Data.TransportLines
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
        ///     Current prefab info index.
        /// </summary>
        [ProtoMember(1)]
        public uint InfoIndex { get; set; }

        /// <summary>
        ///     If SetEditLine should be called with the force parameter.
        /// </summary>
        [ProtoMember(2)]
        public bool ForceSetEditLine { get; set; }

        /// <summary>
        ///     The current temp line id.
        /// </summary>
        [ProtoMember(3)]
        public ushort TempLine { get; set; }

        /// <summary>
        ///     The source line this temp line refers to.
        /// </summary>
        [ProtoMember(4)]
        public ushort SourceLine { get; set; }

        /// <summary>
        ///     A list of line ids that should be released.
        /// </summary>
        [ProtoMember(5)]
        public ushort[] ReleaseLines { get; set; }

        /// <summary>
        ///     If the temp line should be created.
        /// </summary>
        [ProtoMember(6)]
        public bool CreateLine { get; set; }

        /// <summary>
        ///     The index of a previously added stop.
        /// </summary>
        [ProtoMember(7)]
        public int LastAddIndex { get; set; }

        /// <summary>
        ///     The index of a previously moved stop.
        /// </summary>
        [ProtoMember(8)]
        public int LastMoveIndex { get; set; }

        /// <summary>
        ///     The position of a previously moved stop.
        /// </summary>
        [ProtoMember(9)]
        public Vector3 LastMovePos { get; set; }

        /// <summary>
        ///     The position of a previously added stop.
        /// </summary>
        [ProtoMember(10)]
        public Vector3 LastAddPos { get; set; }

        /// <summary>
        ///     The index of the stop that will be added.
        /// </summary>
        [ProtoMember(11)]
        public int AddIndex { get; set; }

        /// <summary>
        ///     The index of the stop that will be moved.
        /// </summary>
        [ProtoMember(12)]
        public int MoveIndex { get; set; }

        /// <summary>
        ///     The position to add a new stop to.
        /// </summary>
        [ProtoMember(13)]
        public Vector3 AddPos { get; set; }

        /// <summary>
        ///     If the line has fixed platforms.
        /// </summary>
        [ProtoMember(14)]
        public bool FixedPlatform { get; set; }

        /// <summary>
        ///     The list of generated Array16 ids.
        /// </summary>
        [ProtoMember(15)]
        public ushort[] Array16Ids { get; set; }
    }
}

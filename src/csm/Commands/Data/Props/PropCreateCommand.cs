using CSM.API.Commands;
using ProtoBuf;
using UnityEngine;

namespace CSM.Commands.Data.Props
{
    /// <summary>
    ///     Called when a prop is created.
    /// </summary>
    /// Sent by:
    /// - PropHandler
    [ProtoContract]
    public class PropCreateCommand : CommandBase
    {
        /// <summary>
        ///     The prop id to create.
        /// </summary>
        [ProtoMember(1)]
        public ushort PropId;

        /// <summary>
        ///     The position of the new prop.
        /// </summary>
        [ProtoMember(2)]
        public Vector3 Position;

        /// <summary>
        ///     The angle of the new prop.
        /// </summary>
        [ProtoMember(3)]
        public float Angle;

        /// <summary>
        ///     If it's a single prop.
        /// </summary>
        [ProtoMember(4)]
        public bool Single;

        /// <summary>
        ///     The info index of the prefab to create.
        /// </summary>
        [ProtoMember(5)]
        public ushort InfoIndex;
    }
}

using ProtoBuf;
using UnityEngine;

namespace CSM.Commands.Data.TransportLines
{
    /// <summary>
    ///     Called when a stop was moved (this tracks calls to the TransportManager and only
    ///     handles calls that are not already caught by the TransportTool injection)
    /// </summary>
    /// Sent by:
    /// - TransportHandler
    [ProtoContract]
    public class TransportLineMoveStopManCommand : CommandBase
    {
        /// <summary>
        ///     The list of generated Array16 ids.
        /// </summary>
        [ProtoMember(1)]
        public ushort[] Array16Ids { get; set; }

        /// <summary>
        ///     The modified line.
        /// </summary>
        [ProtoMember(2)]
        public ushort Line { get; set; }

        /// <summary>
        ///     The new position of the stop.
        /// </summary>
        [ProtoMember(3)]
        public Vector3 NewPos { get; set; }

        /// <summary>
        ///     If the stop has a fixed platform.
        /// </summary>
        [ProtoMember(4)]
        public bool FixedPlatform { get; set; }

        /// <summary>
        ///     The stop index in the line.
        /// </summary>
        [ProtoMember(5)]
        public int Index { get; set; }
    }
}

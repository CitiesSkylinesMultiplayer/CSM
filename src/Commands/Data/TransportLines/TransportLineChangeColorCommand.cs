using ProtoBuf;
using UnityEngine;

namespace CSM.Commands.Data.TransportLines
{
    /// <summary>
    ///     Called when the color of a transport line was changed.
    /// </summary>
    /// Sent by:
    /// - TransportLine
    [ProtoContract]
    public class TransportLineChangeColorCommand : CommandBase
    {
        /// <summary>
        ///     The id of the modified line.
        /// </summary>
        [ProtoMember(1)]
        public ushort LineId { get; set; }

        /// <summary>
        ///     The new color of the line.
        /// </summary>
        [ProtoMember(2)]
        public Color Color { get; set; }
    }
}

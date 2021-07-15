using ProtoBuf;
using UnityEngine;

namespace CSM.Commands.Data.Events
{
    /// <summary>
    ///     Called when the color of an event was changed (e.g. rocket color).
    /// </summary>
    /// Sent by:
    /// - EventHandler
    [ProtoContract]
    public class EventColorChangedCommand : CommandBase
    {
        /// <summary>
        ///     The id of the modified event.
        /// </summary>
        [ProtoMember(1)]
        public ushort Event { get; set; }

        /// <summary>
        ///     The new color.
        /// </summary>
        [ProtoMember(2)]
        public Color Color { get; set; }
    }
}

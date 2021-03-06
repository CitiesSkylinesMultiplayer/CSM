using ProtoBuf;

namespace CSM.Commands.Data.Roads
{
    /// <summary>
    ///     Called when the settings (stop signs, traffic lights) of an intersection are changed.
    /// </summary>
    /// Sent by:
    /// - RoadHandler
    [ProtoContract]
    public class RoadSettingsCommand : CommandBase
    {
        /// <summary>
        ///     The node that was clicked on.
        /// </summary>
        [ProtoMember(1)]
        public ushort NodeId { get; set; }

        /// <summary>
        ///     The index of the button that was clicked on the node.
        /// </summary>
        [ProtoMember(2)]
        public int Index { get; set; }
    }
}

using ProtoBuf;

namespace CSM.Commands.Data.TransportLines
{ 
    /// <summary>
    ///     Called when the day/night settings of a transport line were changed.
    /// </summary>
    /// Sent by:
    /// - TransportHandler
    [ProtoContract]
    public class TransportLineChangeActiveCommand : CommandBase
    {
        /// <summary>
        ///     The id of the modified line
        /// </summary>
        [ProtoMember(1)]
        public ushort LineId;

        /// <summary>
        ///     If the line is active at daytime.
        /// </summary>
        [ProtoMember(2)]
        public bool Day;

        /// <summary>
        ///     If the line is active at nighttime.
        /// </summary>
        [ProtoMember(3)]
        public bool Night;
    }
}

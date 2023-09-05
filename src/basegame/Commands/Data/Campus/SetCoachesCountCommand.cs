using System;
using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Campus
{
    /// <summary>
    ///     Called when the count of varsity coaches is set for a campus.
    /// </summary>
    /// Sent by:
    /// - CampusHandler
    [ProtoContract]
    public class SetCoachesCountCommand : CommandBase
    {
        /// <summary>
        ///     The park id of the campus.
        /// </summary>
        [ProtoMember(1)]
        public byte Campus { get; set; }

        /// <summary>
        ///     The new amount of staff.
        /// </summary>
        [ProtoMember(2)]
        public byte Count { get; set; }

        /// <summary>
        ///     The timestamps when the new coaches were hired.
        /// </summary>
        [ProtoMember(3)]
        public DateTime[] CoachHireTimes { get; set; }
    }
}

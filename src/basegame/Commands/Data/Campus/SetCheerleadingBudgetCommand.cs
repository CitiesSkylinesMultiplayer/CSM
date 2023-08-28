using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Campus
{
    /// <summary>
    ///     Called when the cheerleading budget of a campus is changed.
    /// </summary>
    /// Sent by:
    /// - CampusHandler
    [ProtoContract]
    public class SetCheerleadingBudgetCommand : CommandBase
    {
        /// <summary>
        ///     The park id of the campus.
        /// </summary>
        [ProtoMember(1)]
        public byte Campus { get; set; }

        /// <summary>
        ///     The new cheerleading budget.
        /// </summary>
        [ProtoMember(2)]
        public int Budget { get; set; }
    }
}

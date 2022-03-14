using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Zones
{
    /// <summary>
    ///     This sends the demand displayed on the UI.
    /// </summary>
    /// Sent by:
    /// - DemandExtension
    [ProtoContract]
    public class DemandDisplayedCommand : CommandBase
    {
        /// <summary>
        ///     The demand for residential areas.
        /// </summary>
        [ProtoMember(1)]
        public int ResidentialDemand { get; set; }

        /// <summary>
        ///     The demand for commercial areas.
        /// </summary>
        [ProtoMember(2)]
        public int CommercialDemand { get; set; }

        /// <summary>
        ///     The demand for workplaces.
        /// </summary>
        [ProtoMember(3)]
        public int WorkplaceDemand { get; set; }
    }
}

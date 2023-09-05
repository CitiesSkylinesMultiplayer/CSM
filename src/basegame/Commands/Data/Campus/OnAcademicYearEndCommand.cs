using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Campus
{
    /// <summary>
    ///     Called when the academic year of a campus area ends.
    /// </summary>
    /// Sent by:
    /// - CampusHandler
    [ProtoContract]
    public class OnAcademicYearEndCommand : CommandBase
    {
        /// <summary>
        ///     The park id of the campus area.
        /// </summary>
        [ProtoMember(1)]
        public byte ParkId { get; set; }

        /// <summary>
        ///     Information regarding the academic works.
        /// </summary>
        [ProtoMember(2)]
        public AcademicWorksData AcademicWorksData { get; set; }

        /// <summary>
        ///     Statistical data of the campus area used in the academic year info panel.
        /// </summary>
        [ProtoMember(3)]
        public DistrictYearReportData[] LedgerReports { get; set; }

        /// <summary>
        ///     The new dynamic attractiveness modifier.
        /// </summary>
        [ProtoMember(4)]
        public int DynamicAttractiveness { get; set; }

        /// <summary>
        ///     The park level of the previous year.
        /// </summary>
        [ProtoMember(5)]
        public DistrictPark.ParkLevel OldParkLevel { get; set; }

        /// <summary>
        ///     The park level in the following year.
        /// </summary>
        [ProtoMember(6)]
        public DistrictPark.ParkLevel NewParkLevel { get; set; }
    }
}

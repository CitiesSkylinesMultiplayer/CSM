using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Campus
{
    [ProtoContract]
    public class OnAcademicYearEndCommand : CommandBase
    {
        [ProtoMember(1)]
        public byte ParkId { get; set; }

        [ProtoMember(2)]
        public AcademicWorksData AcademicWorksData { get; set; }

        [ProtoMember(3)]
        public DistrictYearReportData[] LedgerReports { get; set; }

        [ProtoMember(4)]
        public int DynamicAttractiveness { get; set; }

        [ProtoMember(5)]
        public DistrictPark.ParkLevel OldParkLevel { get; set; }

        [ProtoMember(6)]
        public DistrictPark.ParkLevel NewParkLevel { get; set; }
    }
}

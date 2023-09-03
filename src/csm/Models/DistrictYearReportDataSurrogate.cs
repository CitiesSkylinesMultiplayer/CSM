using CSM.API.Helpers;
using ProtoBuf;
// ReSharper disable InconsistentNaming

namespace CSM.Models
{
    [ProtoContract]
    public class DistrictYearReportDataSurrogate
    {
        [ProtoMember(1)]
        private byte m_reputationLevel;
        [ProtoMember(2)]
        private int m_studentCount;
        [ProtoMember(3)]
        private int m_campusAttractiveness;
        [ProtoMember(4)]
        private int m_totalWorks;
        [ProtoMember(5)]
        private byte m_grantType;
        [ProtoMember(6)]
        private int m_grantWorkIndex;
        [ProtoMember(7)]
        private AcademicWorksData m_academicWorksData;
        [ProtoMember(8)]
        private ulong m_totalPrizeMoney;
        [ProtoMember(9)]
        private byte m_togaPartyCount;
        [ProtoMember(10)]
        private long m_togaPartyCurrentSeed;
        [ProtoMember(11)]
        private long m_togaPartySeed1;
        [ProtoMember(12)]
        private long m_togaPartySeed2;
        [ProtoMember(13)]
        private string m_milestoneUnlocked;
        [ProtoMember(14)]
        private ulong[] m_ticketsIncome;
        [ProtoMember(15)]
        private ulong[] m_winningsIncome;
        [ProtoMember(16)]
        private byte[] m_matchesWon;
        [ProtoMember(17)]
        private byte[] m_matchesLost;
        [ProtoMember(18)]
        private byte[] m_matchesCancelled;
        [ProtoMember(19)]
        private bool[] m_trophies;

        public static implicit operator DistrictYearReportDataSurrogate(DistrictYearReportData value)
        {
            return new DistrictYearReportDataSurrogate
            {
                m_reputationLevel = value.m_reputationLevel,
                m_studentCount = value.m_studentCount,
                m_campusAttractiveness = value.m_campusAttractiveness,
                m_totalWorks = value.m_totalWorks,
                m_grantType = value.m_grantType,
                m_grantWorkIndex = value.m_grantWorkIndex,
                m_academicWorksData = value.m_academicWorksData,
                m_totalPrizeMoney = value.m_totalPrizeMoney,
                m_togaPartyCount = value.m_togaPartyCount,
                m_togaPartyCurrentSeed = value.m_togaPartyCurrentSeed,
                m_togaPartySeed1 = value.m_togaPartySeed1,
                m_togaPartySeed2 = value.m_togaPartySeed2,
                m_milestoneUnlocked = value.m_milestoneUnlocked,
                m_ticketsIncome = ReflectionHelper.GetAttr<ulong[]>(value, "m_ticketsIncome"),
                m_winningsIncome = ReflectionHelper.GetAttr<ulong[]>(value, "m_winningsIncome"),
                m_matchesWon = ReflectionHelper.GetAttr<byte[]>(value, "m_matchesWon"),
                m_matchesLost = ReflectionHelper.GetAttr<byte[]>(value, "m_matchesLost"),
                m_matchesCancelled = ReflectionHelper.GetAttr<byte[]>(value, "m_matchesCancelled"),
                m_trophies = ReflectionHelper.GetAttr<bool[]>(value, "m_trophies"),
            };
        }

        public static implicit operator DistrictYearReportData(DistrictYearReportDataSurrogate value)
        {
            DistrictYearReportData data = new DistrictYearReportData
            {
                m_reputationLevel = value.m_reputationLevel,
                m_studentCount = value.m_studentCount,
                m_campusAttractiveness = value.m_campusAttractiveness,
                m_totalWorks = value.m_totalWorks,
                m_grantType = value.m_grantType,
                m_grantWorkIndex = value.m_grantWorkIndex,
                m_academicWorksData = value.m_academicWorksData,
                m_totalPrizeMoney = value.m_totalPrizeMoney,
                m_togaPartyCount = value.m_togaPartyCount,
                m_togaPartyCurrentSeed = value.m_togaPartyCurrentSeed,
                m_togaPartySeed1 = value.m_togaPartySeed1,
                m_togaPartySeed2 = value.m_togaPartySeed2,
                m_milestoneUnlocked = value.m_milestoneUnlocked,
            };
            value.m_ticketsIncome?.CopyTo(data.ticketsIncome, 0);
            value.m_winningsIncome?.CopyTo(data.winningsIncome, 0);
            value.m_matchesWon?.CopyTo(data.matchesWon, 0);
            value.m_matchesLost?.CopyTo(data.matchesLost, 0);
            value.m_matchesCancelled?.CopyTo(data.matchesCancelled, 0);
            value.m_trophies?.CopyTo(data.trophies, 0);

            return data;
        }
    }
}

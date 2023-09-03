using ProtoBuf;

namespace CSM.Models
{
    [ProtoContract]
    public class AcademicWorksDataSurrogate
    {
        [ProtoMember(1)]
        public int m_worksType1 { get; set; }

        [ProtoMember(2)]
        public int m_worksType2 { get; set; }

        [ProtoMember(3)]
        public int m_worksType3 { get; set; }

        [ProtoMember(4)]
        public int WorksCount { get; set; }

        public static implicit operator AcademicWorksDataSurrogate(AcademicWorksData value)
        {
            return new AcademicWorksDataSurrogate
            {
                m_worksType1 = value.m_worksType1,
                m_worksType2 = value.m_worksType2,
                m_worksType3 = value.m_worksType3,
                WorksCount = value.m_worksCount
            };
        }

        public static implicit operator AcademicWorksData(AcademicWorksDataSurrogate value)
        {
            return new AcademicWorksData
            {
                m_worksType1 = value.m_worksType1,
                m_worksType2 = value.m_worksType2,
                m_worksType3 = value.m_worksType3,
                m_worksCount = value.WorksCount
            };
        }
    }
}

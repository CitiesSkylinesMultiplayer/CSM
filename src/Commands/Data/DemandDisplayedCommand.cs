using ProtoBuf;

namespace CSM.Commands
{
    /// <summary>
    /// This sends the demand displayed on the UI
    /// </summary>
    [ProtoContract]
    public class DemandDisplayedCommand : CommandBase
    {
        [ProtoMember(1)]
        public int ResidentialDemand { get; set; }

        [ProtoMember(2)]
        public int CommercialDemand { get; set; }

        [ProtoMember(3)]
        public int WorkplaceDemand { get; set; }
    }
}
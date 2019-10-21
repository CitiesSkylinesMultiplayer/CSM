using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public class TaxRateChangeCommand : CommandBase
    {
        [ProtoMember(1)]
        public int[] TaxRate { get; set; }
    }
}

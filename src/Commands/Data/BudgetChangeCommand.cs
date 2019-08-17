using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public class BudgetChangeCommand : CommandBase
    {
        [ProtoMember(1)]
        public int[] ServiceBudgetDay { get; set; }

        [ProtoMember(2)]
        public int[] ServiceBudgetNight { get; set; }
    }
}

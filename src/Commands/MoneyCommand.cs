using ProtoBuf;

namespace CSM.Commands
{
    /// <summary>
    ///     This sends the current cash amount
    ///    TODO: find out how top copy an array to reflection
    /// /// </summary>
    [ProtoContract]
    public class MoneyCommand : CommandBase
    {
        [ProtoMember(1)]
        public long InternalMoneyAmount { get; set; }

        [ProtoMember(2)]
        public long[] TotalIncome { get; set; }

        [ProtoMember(3)]
        public long[] TotalExpenses { get; set; }

		[ProtoMember(4)]
		public int[] Taxrate { get; set; }

		[ProtoMember(5)]
		public int[] ServiceBudgetNight { get; set; }

		[ProtoMember(6)]
		public int[] ServiceBudgetDay { get; set; }

		
	}
}
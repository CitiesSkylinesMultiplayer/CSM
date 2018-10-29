using ProtoBuf;

namespace CSM.Commands
{
	[ProtoContract]
	public class TaxRateChangeCommand : CommandBase
	{
		[ProtoMember(1)]
		public int[] Taxrate { get; set; }

	}
}

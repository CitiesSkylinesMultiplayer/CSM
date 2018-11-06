using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
	[ProtoContract]
	public class BuildingIDCommand : CommandBase
	{
		[ProtoMember(1)]
		public uint BuildingIDSender { get; set; }

		[ProtoMember (2)]
		public uint BuildingIDReciever { get; set; }
	}
}
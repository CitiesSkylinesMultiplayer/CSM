using ProtoBuf;
using UnityEngine;


namespace CSM.Commands
{
	[ProtoContract]
	public class ZoneCommand : CommandBase
	{
		[ProtoMember(1)]
		public Vector3 Position { get; set; }

		[ProtoMember(2)]
		public ulong Zone1 { get; set; }

		[ProtoMember(3)]
		public ulong Zone2 { get; set; }

	}
}
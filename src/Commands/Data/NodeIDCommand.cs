using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
	[ProtoContract]
	class NodeIDCommand : CommandBase
	{
		[ProtoMember(1)]
		public uint NodeIDSender { get; set; }

		[ProtoMember(2)]
		public uint NodeIDReciever { get; set; }
	}
}

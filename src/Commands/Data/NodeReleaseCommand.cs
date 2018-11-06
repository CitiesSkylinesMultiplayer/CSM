using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
	[ProtoContract]
	public class NodeReleaseCommand : CommandBase
	{
		[ProtoMember(1)]
		public ushort Nodeid { get; set; }

	}
}
using ProtoBuf;
using System.IO;
using CSM.Helpers;
using UnityEngine;
using System.Runtime.Serialization;
using ProtoBuf.Meta;


namespace CSM.Commands
{
	[ProtoContract]
	public class RoadCommand : CommandBase
	{
		[ProtoMember(1)]
		public ushort Startnode { get; set; }
		[ProtoMember(2)]
		public ushort Endnode { get; set; }
		[ProtoMember(3)]
		public Vector3 StartDirection { get; set; }
		[ProtoMember(4)]
		public Vector3 Enddirection { get; set; }
		[ProtoMember(5)]
		public uint ModifiedIndex { get; set; }
		[ProtoMember(6)]
		public ushort InfoIndex { get; set; }

		public static RoadCommand Deserialize(byte[] message)
		{
			RoadCommand result;

			using (var stream = new MemoryStream(message))
			{
				result = Serializer.Deserialize<RoadCommand>(stream);
			}

			return result;
		}



	}
}

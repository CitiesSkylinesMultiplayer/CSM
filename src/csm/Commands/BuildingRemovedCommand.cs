using ProtoBuf;
using System.IO;
using CSM.Helpers;
using UnityEngine;
using System.Runtime.Serialization;
using ProtoBuf.Meta;

namespace CSM.Commands
{
	[ProtoContract]
	public class BuildingRemovedCommand : CommandBase
	{
		[ProtoMember(1)]
		public Vector3 position { get; set; }


		public static BuildingRemovedCommand Deserialize(byte[] message)
		{
			BuildingRemovedCommand result;

			using (var stream = new MemoryStream(message))
			{
				result = Serializer.Deserialize<BuildingRemovedCommand>(stream);
			}

			return result;
		}
	}
}

using ProtoBuf;
using UnityEngine;
namespace CSM.Helpers
{   [ProtoContract]
	public class Vector3Surrogate
	{
		[ProtoMember(1)]
		public float x { get; set; }
		[ProtoMember(2)]
		public float y { get; set; }
		[ProtoMember(3)]
		public float z { get; set; }

		public static implicit operator Vector3Surrogate(Vector3 value)
		{
			return new Vector3Surrogate
			{
				x = value.x,
				y = value.y,
				z = value.z
			};
		}

		public static implicit operator Vector3(Vector3Surrogate value)
		{
			return new Vector3 { x = value.x, y = value.y, z = value.z };
		}
	}

}

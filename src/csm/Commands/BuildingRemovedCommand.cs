using ProtoBuf;
using System.IO;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class BuildingRemovedCommand : CommandBase
    {
        [ProtoMember(1)]
        public Vector3 Position { get; set; }

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
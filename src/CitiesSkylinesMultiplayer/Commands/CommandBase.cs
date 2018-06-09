using System.IO;
using ProtoBuf;

namespace CitiesSkylinesMultiplayer.Commands
{
    [ProtoContract]
    public abstract class CommandBase
    {
        public byte[] Serialize()
        {
            byte[] result;

            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, this);
                result = stream.ToArray(); 
            }

            return result;
        }
    }
}
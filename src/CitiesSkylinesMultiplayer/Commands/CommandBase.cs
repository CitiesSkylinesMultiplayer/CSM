using System.IO;
using ProtoBuf;

namespace CitiesSkylinesMultiplayer.Commands
{
    [ProtoContract]
    public abstract class CommandBase
    {
        public const byte ConnectionRequestCommand = 0;
        public const byte ConnectionResultCommand = 0;

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
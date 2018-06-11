using System.CodeDom;
using System.IO;
using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public abstract class CommandBase
    {
        public const byte ConnectionRequestCommandId = 0;
        public const byte ConnectionResultCommandId = 1;
        public const byte PingCommandId = 3;

        public const byte SimulationCommandID = 100;



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
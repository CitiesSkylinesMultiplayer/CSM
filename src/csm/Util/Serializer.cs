using CSM.API.Commands;
using CSM.Commands;
using System.IO;

namespace CSM.Util
{
    public static class Serializer
    {
        /// <summary>
        ///     Serializes the command into a byte array for sending over the network.
        /// </summary>
        /// <returns>A byte array containing the message.</returns>
        public static byte[] Serialize(CommandBase cmd)
        {
            byte[] result;

            using (MemoryStream stream = new MemoryStream())
            {
                Command.Model.Serialize(stream, cmd);
                result = stream.ToArray();
            }

            return result;
        }
    }
}

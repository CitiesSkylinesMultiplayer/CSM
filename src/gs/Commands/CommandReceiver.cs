using System.IO;
using System.Net;
using CSM.GS.Commands.Data.ApiServer;
using CSM.GS.Commands.Handler.ApiServer;
using LiteNetLib;

namespace CSM.GS.Commands
{
    public static class CommandReceiver
    {
        /// <summary>
        ///     This method is used to parse an incoming message from the API server
        ///     and execute the appropriate action.
        /// </summary>
        /// <param name="endPoint">The sending IP end point</param>
        /// <param name="reader">The incoming packet including the command type byte.</param>
        /// <param name="service">The worker service instance</param>
        public static void Parse(WorkerService service, IPEndPoint endPoint, NetPacketReader reader)
        {
            ApiCommandBase cmd = Deserialize(reader.GetRemainingBytes());


            ApiCommandHandler handler = ApiCommand.Instance.GetApiCommandHandler(cmd.GetType());

            handler?.Parse(service, endPoint, cmd);
        }

        /// <summary>
        ///     Deserialize the command from a byte array.
        /// </summary>
        /// <param name="message">A byte array of the message</param>
        /// <returns>The deserialized command.</returns>
        private static ApiCommandBase Deserialize(byte[] message)
        {
            using MemoryStream stream = new(message);
            ApiCommandBase result = (ApiCommandBase)ApiCommand.Instance.ApiModel.Deserialize(stream, null, typeof(ApiCommandBase));

            return result;
        }
    }
}

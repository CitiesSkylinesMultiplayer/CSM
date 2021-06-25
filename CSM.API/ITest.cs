using CSM.API.Commands;
using System;

namespace CSM.API
{

    public interface ITest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="function"> The function to be called to send commands</param>
        /// <returns>true if successful</returns>
        bool ConnectToCSM(Func<CommandBase, bool> function);

        /// <summary>
        /// Gets a unique identifier for this handler.  Only one handler can be loaded with a given identifier.
        /// </summary>
        Guid HandlerID { get; }

        string name { get; }

        /// <summary>
        /// Handles the specified request.  The method should not close the stream.
        /// </summary>
        String Handle(byte[] data);

    }

}

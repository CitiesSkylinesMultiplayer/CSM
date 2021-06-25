using System;
using System.Collections.Generic;
using System.Text;

namespace CSM.API
{

    public interface ITest
    {
        bool ConnectToCSM(Func<string, byte[], bool> function);
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

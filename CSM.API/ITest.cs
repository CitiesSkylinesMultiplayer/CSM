using System;
using System.Collections.Generic;
using System.Text;

namespace CSM.API
{

    public interface ITest
    {

        /// <summary>
        /// Gets a unique identifier for this handler.  Only one handler can be loaded with a given identifier.
        /// </summary>
        Guid HandlerID { get; }

        /// <summary>
        /// Handles the specified request.  The method should not close the stream.
        /// </summary>
        String Handle();

    }

}

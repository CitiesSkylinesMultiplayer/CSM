using System;
using System.Collections.Generic;
using System.Text;

namespace CSM.API
{

    interface ITest
    {

        /// <summary>
        /// Handles the specified request.  The method should not close the stream.
        /// </summary>
        String Handle();

    }

}

using System;
using System.Collections.Generic;
using System.Text;

namespace CSM.API
{

    public interface ITest
    {

        /// <summary>
        /// Handles the specified request.  The method should not close the stream.
        /// </summary>
        String Handle();

    }

}

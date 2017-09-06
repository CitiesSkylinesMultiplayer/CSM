using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Networking
{
    public class Client : IDisposable
    {
        #region Setup
        private static Client _serverInstance;
        public static Client Instance => _serverInstance ?? (_serverInstance = new Client());
        #endregion


        public void Dispose()
        {
          
        }

        ~Client()
        {
            Dispose();
        }
    }
}

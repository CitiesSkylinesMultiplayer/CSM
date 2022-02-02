using CSM.StandaloneServer.Networking;
using CSM.Networking.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSM.StandaloneServer
{
    class Program
    {
        static void Main(string[] args)
        {
            BaseServerConfig baseServerConfig = new ServerConfig(4230, null, "", 10);
            Host.Instance.StartGameServer((ServerConfig)baseServerConfig, () => {
                Console.WriteLine("Server started");
            });
        }
    }
}

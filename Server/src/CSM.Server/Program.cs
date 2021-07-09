using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Config;
using CSM.Server.Util;
using System;
using System.Diagnostics;
using System.Threading;

namespace CSM.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Initialize(true);

            var serverConfig = new ServerConfig()
            {   
            };
            MultiplayerManager.Instance.StartGameServer(serverConfig, (success) =>
            {
                if (!success)
                {
                    Console.WriteLine("Could not start server.");
                }
            });

            while (true)
            {
                SpeedPauseHelper.SimulationStep();
                MultiplayerManager.Instance.ProcessEvents();
                Thread.Sleep(15);
            }
        }
    }
}

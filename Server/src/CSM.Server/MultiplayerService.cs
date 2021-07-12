using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Config;
using CSM.Server.Util;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace CSM.Server
{
    class MultiplayerService : IHostedService
    {


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var serverConfig = new ServerConfig()
            {
            };
            MultiplayerManager.Instance.StartGameServer(serverConfig, (success) =>
            {
                if (!success)
                {
                    Log.Error("Could not start server.");
                }
            });
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            MultiplayerManager.Instance.StopEverything();
        }
    }
}

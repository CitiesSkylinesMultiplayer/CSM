using CSM.Networking;
using CSM.Networking.Config;
using CSM.Server.Util;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CSM.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Initialize(true);

            var stoppingTokenSource = new CancellationTokenSource();
            var host = CreateHostBuilder(args);

            var serverConfig = new ServerConfig()
            {   
            };

            MultiplayerManager.Instance.StartGameServer(serverConfig, (success) =>
            {
                if (!success)
                {
                    Log.Error("Could not start server.");
                    stoppingTokenSource.Cancel();
                }
            });

            await host.RunConsoleAsync(stoppingTokenSource.Token);
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                   .ConfigureServices(ConfigureServices);

        private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddHostedService<UpdateLoopService>();
        }
    }
}

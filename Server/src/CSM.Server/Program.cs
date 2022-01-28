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
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                   .AddCSMServer();
    }

    public static class HostBuilderExtensions
    {

        public static IHostBuilder AddCSMServer(this IHostBuilder builder) => builder
            .ConfigureServices(services =>
            {
                services.AddHostedService<MultiplayerService>();
                services.AddHostedService<UpdateLoopService>();
            });
    }
}

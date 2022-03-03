using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Net;
using Distel.OrleansProviders;

namespace Distel.WebHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
            .UseOrleans(siloBuilder =>
            {
                siloBuilder
                .ConfigureEndpoints(IPAddress.Loopback, siloPort: 11111, gatewayPort: 30000, listenOnAnyHostAddress: true)
                .UseLocalhostClustering()
                .AddMemoryGrainStorageAsDefault()
                .AddSimpleMessageStreamProvider("attractions-stream")
                .AddMemoryGrainStorage("PubSubStore")
                .Configure<ClusterOptions>(opts =>
                {
                    opts.ClusterId = "dev";
                    opts.ServiceId = "DistelAPI";
                })
                .Configure<EndpointOptions>(opts =>
                {
                    opts.AdvertisedIPAddress = IPAddress.Loopback;
                });
            });
    }
}

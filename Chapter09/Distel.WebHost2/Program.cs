using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Net;

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
                webBuilder.UseUrls("https://localhost:6000");
            })
        .UseOrleans(siloBuilder =>
        {
            siloBuilder
            .ConfigureEndpoints(IPAddress.Loopback, siloPort: 11112, gatewayPort: 30001, listenOnAnyHostAddress: true)
            .UseAzureStorageClustering((options) =>
                {
                    options.TableName = "distelcluster";
                    options.ConnectionString = "<<Azure table storage conneciton string>>";
                })
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

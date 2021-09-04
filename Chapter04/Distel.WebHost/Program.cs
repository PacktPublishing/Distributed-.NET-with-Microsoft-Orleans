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
                })
            .UseOrleans(siloBuilder =>
            {
                siloBuilder
                .UseLocalhostClustering()
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

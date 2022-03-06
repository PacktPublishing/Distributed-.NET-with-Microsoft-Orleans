using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
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
                int siloPort = 11111;
                int gatewayPort = 22222;
                if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("WEBSITE_PRIVATE_PORTS")))
                {
                    var strPorts = Environment.GetEnvironmentVariable("WEBSITE_PRIVATE_PORTS").Split(',');
                    if (strPorts.Length >= 2)
                    {
                        siloPort = int.Parse(strPorts[0]);
                        gatewayPort = int.Parse(strPorts[1]);
                    }
                }
                siloBuilder
                .ConfigureEndpoints(IPAddress.Parse(Environment.GetEnvironmentVariable("WEBSITE_PRIVATE_IP")), siloPort: siloPort, gatewayPort: gatewayPort, listenOnAnyHostAddress: true)
                .UseCosmosDBMembership(options =>
                {
                    options.AccountEndpoint = "";
                    options.AccountKey = "";
                    options.Collection = "Membership";
                    options.CanCreateResources = true;
                })
                .AddCosmosDBGrainStorageAsDefault(options =>
                {
                    options.AccountEndpoint = "";
                    options.AccountKey = "";
                    options.Collection = "State";
                    options.CanCreateResources = true;
                })
                //.UseLocalhostClustering()
                //.AddMemoryGrainStorageAsDefault()
                .AddSimpleMessageStreamProvider("attractions-stream")
                .AddMemoryGrainStorage("PubSubStore")
                .Configure<ClusterOptions>(opts =>
                {
                    opts.ClusterId = Environment.GetEnvironmentVariable("REGION_NAME");
                    opts.ServiceId = "DistelAPI";
                })
                .UseDashboard( options=> {
                    options.HostSelf = false;
                });
                //.Configure<EndpointOptions>(opts =>
                //{
                //    opts.AdvertisedIPAddress = IPAddress.Parse(Environment.GetEnvironmentVariable("WEBSITE_PRIVATE_IP"));
                //});
            });
    }
}

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
                .UseLocalhostClustering()
                // Register Cosmos DB provider as the default stirage provider
                .AddCosmosDBGrainStorageAsDefault(opt =>
                {
                    opt.AccountEndpoint = "https://localhost:8081";
                    opt.AccountKey = "<<Update cosmos DB Account Key>>";
                    opt.DB = "distelstore";
                    opt.CanCreateResources = true;
                })
                .AddAzureFileGrainStorage("FileShare", opt => {
                    opt.Share = "distelstore";
                    opt.ConnectionString = "<<Update Storage connection string>>";
                })
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

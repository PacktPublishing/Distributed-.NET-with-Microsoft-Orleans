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
                    opt.AccountKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
                    opt.DB = "distelstore";
                    opt.CanCreateResources = true;
                })
                .AddAzureFileGrainStorage("FileShare", opt => {
                    opt.Share = "distelstore";
                    opt.ConnectionString = "DefaultEndpointsProtocol=https;AccountName=silostorage;AccountKey=XAJiRm2mEYFM79KvZDTp2oVbELQkhGnDiV9eBLzmK7Q3E1q1HUbBsRY7U2+1aUUkM8e/T8lobEG9QdUzhEzXQQ==;EndpointSuffix=core.windows.net";
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

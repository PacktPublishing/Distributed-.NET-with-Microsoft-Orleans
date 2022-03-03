using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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
                }).UseOrleans(siloBuilder =>
                {
                    siloBuilder
                    .UseLocalhostClustering()
                    .UseInMemoryReminderService()
                    .AddCosmosDBGrainStorageAsDefault(opt =>
                    {
                        opt.AccountEndpoint = "<<Account End point>>";
                        opt.AccountKey = "<<Account Key>>";
                        opt.DB = "<<DB Name>>";
                        opt.CanCreateResources = true;
                    })
                    .Configure<ClusterOptions>(opts =>
                    {
                        opts.ClusterId = "dev";
                        opts.ServiceId = "DistelService";
                    })
                    .Configure<EndpointOptions>(opts =>
                    {
                        opts.AdvertisedIPAddress = IPAddress.Loopback;
                    });
                });            
    }
}

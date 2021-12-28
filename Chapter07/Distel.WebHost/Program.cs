using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
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
                    .ConfigureApplicationParts(parts => parts.AddFromApplicationBaseDirectory())                    
                    .UseDashboard(options => { })
                    .UseLocalhostClustering()
                    .UseInMemoryReminderService()                    
                    .AddCosmosDBGrainStorageAsDefault(opt =>
                    {
                        opt.AccountEndpoint = "<<Account endpoint>>";
                        opt.AccountKey = "<<Account key>>";
                        opt.DB = "testpersistentgrainstorage";
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

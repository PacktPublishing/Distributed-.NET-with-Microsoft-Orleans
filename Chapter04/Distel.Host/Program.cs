using Distel.Grains;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Distel.Host
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                using (var host = await StartSilo())
                {
                    Console.WriteLine("\n\n Press any key to terminate...\n\n");
                    Console.ReadKey();

                    await host.StopAsync();
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static async Task<IHost> StartSilo()
        {
            var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .UseOrleans(builder=> {
                    builder.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "dev";
                        options.ServiceId = "DistelService";
                    })
                    .UseLocalhostClustering(11111, 30000)
                    .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(HotelGrain).Assembly).WithReferences())
                    .ConfigureLogging(logging => logging.AddConsole());
                });

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}

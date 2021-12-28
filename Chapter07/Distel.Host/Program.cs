using Distel.Grains;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Distel.Host
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var host = await StartSilo();
            Console.WriteLine("\n\n Press any key to terminate...\n\n");
            Console.ReadKey();

            await host.StopAsync();

            return 0;
        }

        private static async Task<ISiloHost> StartSilo()
        {
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "DistelService";
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(HotelGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole());
            var host = builder.Build();
            await host.StartAsync();
            return host;
        }

    }
}

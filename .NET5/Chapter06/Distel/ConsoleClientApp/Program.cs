using Distel.Grains.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using System;
using System.Threading.Tasks;

namespace ConsoleClientApp
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                using (var client = await ConnectClient())
                {
                    while (true)
                    {
                        Console.WriteLine("Please enter guest name. Type 'exit' to close: ");
                        var guest = Console.ReadLine();
                        if (guest == "exit")
                            break;
                        await SendWelcomeGreeting(client, guest);
                    }
                }
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nException while trying to run client: {e.Message}");
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
                return 1;
            }
        }


        private static async Task<IClusterClient> ConnectClient()
        {
            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "DistelService";
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect();
            Console.WriteLine("Client successfully connected to silo host \n");
            return client;
        }

        private static async Task SendWelcomeGreeting(IClusterClient client, string guest)
        {
            var hotel = client.GetGrain<IHotelGrain>("Taj");
            Console.WriteLine("Identity String : " + hotel.GetGrainIdentity().IdentityString);
            var response = await hotel.WelcomeGreetingAsync(guest);           
            Console.WriteLine($"\n\n{response}\n");
            HotelClient c = new HotelClient();            
            var obj = await client.CreateObjectReference<IObserver>(c);
            await hotel.Subscribe(obj);
        }


    }
}

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
                    //var discountWorker = client.GetGrain<IDiscountCalculator>(0);
                    //var discount = await discountWorker.ComputeDiscount(150);
                    //Console.WriteLine($"Discount for the Amount ${150} is ${discount}");

                    while (true)
                    {
                        Console.Write("Please enter guest name. Type 'exit' to close: ");
                        var guest = Console.ReadLine();

                        if (guest == "exit")
                            break;
                        await SendWelcomeGreeting(client, guest);
                    }

                    await client.Close();
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
                .UseLocalhostClustering(30000)
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
            //for (int i = 0; i < 1000; i++)
            {

                var hotel = client.GetGrain<IHotelGrain>(Guid.NewGuid().ToString());
                //var hotel1 = client.GetGrain<IHotelGrain>("Distel.Agra");

                //Console.WriteLine("Hotel Grain PrimaryKey : " + await hotel.GetKey());
                //Console.WriteLine("Identity String : " + hotel.GetGrainIdentity().IdentityString);
                //Console.WriteLine("Identity String : " + hotel1.GetGrainIdentity().IdentityString);

                var response = await hotel.WelcomeGreetingAsync(guest);
                Console.WriteLine($"\n\n{response}\n\n");
            }
        }
    }
}

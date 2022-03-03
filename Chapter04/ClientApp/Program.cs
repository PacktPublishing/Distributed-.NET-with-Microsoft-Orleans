using Distel.Grains.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using System;
using System.Threading.Tasks;



IClusterClient client = new ClientBuilder()
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

while (true)
{
    Console.Write("Please enter guest name. Type 'exit' to close: ");
    var guest = Console.ReadLine();

    if (guest == "exit")
        break;
    await SendWelcomeGreeting(client, guest);
}

Console.WriteLine("Closing the client \n");
await client.Close();
client.Dispose();



async Task SendWelcomeGreeting(IClusterClient client, string guest)
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


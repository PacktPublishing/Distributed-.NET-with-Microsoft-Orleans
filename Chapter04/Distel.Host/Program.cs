using Distel.Grains;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

// Configure the host builder to host Orleans silo
IHost host = Host.CreateDefaultBuilder(args)
    .UseOrleans(( builder) =>
    {
        builder.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "dev";
            options.ServiceId = "DistelService";
        })
        .UseLocalhostClustering(11111, 30000)
        .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(HotelGrain).Assembly).WithReferences())
        .ConfigureLogging(logging => logging.AddConsole());
    })
    .Build();

await host.RunAsync();

// Dispose the host
host.Dispose();
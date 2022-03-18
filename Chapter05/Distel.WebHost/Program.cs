using Distel.OrleansProviders;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseOrleans(siloBuilder =>
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Distel.WebHost v1"));
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
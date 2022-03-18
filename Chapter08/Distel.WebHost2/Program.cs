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
    //.UseLocalhostClustering()
    // Register Cosmos DB provider as the default stirage provider
    .ConfigureEndpoints(IPAddress.Loopback, siloPort: 11112, gatewayPort: 30001, listenOnAnyHostAddress: true)
    .AddCosmosDBGrainStorageAsDefault(opt =>
    {
        opt.AccountEndpoint = "https://localhost:8081";
        opt.AccountKey = "<<Cosmos DB Key>>";
        opt.DB = "distelstore";
        opt.CanCreateResources = true;
    })
    .AddAzureFileGrainStorage("FileShare", opt =>
    {
        opt.Share = "distelstore";
        opt.ConnectionString = "<<Azure storage connection string>>";
    })
    .UseAzureStorageClustering((options) =>
    {
        options.TableName = "distelcluster";
        options.ConfigureTableServiceClient("<<Azure storage connection string>>");
    })
    .AddMemoryGrainStorageAsDefault()
    .AddSimpleMessageStreamProvider("attractions-stream")
    .AddMemoryGrainStorage("PubSubStore")
    .UseInMemoryReminderService()
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

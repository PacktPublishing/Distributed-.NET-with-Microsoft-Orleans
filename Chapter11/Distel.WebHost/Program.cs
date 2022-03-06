using Distel.DataTier;
using Distel.DataTier.Abstractions;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddApplicationInsightsTelemetry(Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseOrleans(siloBuilder =>
{
    int siloPort = 11111;
    int gatewayPort = 22222;
    if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("WEBSITE_PRIVATE_PORTS")))
    {
        var strPorts = Environment.GetEnvironmentVariable("WEBSITE_PRIVATE_PORTS").Split(',');
        if (strPorts.Length >= 2)
        {
            siloPort = int.Parse(strPorts[0]);
            gatewayPort = int.Parse(strPorts[1]);
        }
    }
    siloBuilder
    .ConfigureEndpoints(IPAddress.Parse(Environment.GetEnvironmentVariable("WEBSITE_PRIVATE_IP")), siloPort: siloPort, gatewayPort: gatewayPort, listenOnAnyHostAddress: true)
    .UseCosmosDBMembership(options =>
    {
        options.AccountEndpoint = "<<Cosmos DB endpoint>>";
        options.AccountKey = "<<CosmosDB account key>>";
        options.Collection = "Membership";
        options.CanCreateResources = true;
    })
    .AddCosmosDBGrainStorageAsDefault(options =>
    {
        options.AccountEndpoint = "<<Cosmos DB endpoint>>";
        options.AccountKey = "<<CosmosDB account key>>";
        options.Collection = "State";
        options.CanCreateResources = true;
    })
    //.UseLocalhostClustering()
    //.AddMemoryGrainStorageAsDefault()
    .AddSimpleMessageStreamProvider("attractions-stream")
    .AddMemoryGrainStorage("PubSubStore")
    .Configure<ClusterOptions>(opts =>
    {
        opts.ClusterId = Environment.GetEnvironmentVariable("REGION_NAME");
        opts.ServiceId = "DistelAPI";
    })
    .ConfigureLogging(logging => logging.AddApplicationInsights(Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY")))
    .UseDashboard(options => {
        options.HostSelf = false;
    });
});

var app = builder.Build();

//if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Distel.WebHost v1"));
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



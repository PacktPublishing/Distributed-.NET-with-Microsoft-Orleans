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
    //siloBuilder
    //.UseLocalhostClustering()
    //.Configure<ClusterOptions>(opts =>
    //{
    //    opts.ClusterId = "dev";
    //    opts.ServiceId = "DistelAPI";
    //})
    //.Configure<EndpointOptions>(opts =>
    //{
    //    opts.AdvertisedIPAddress = IPAddress.Loopback;
    //});


    // In Kubernetes, we use environment variables and the pod manifest
    siloBuilder.UseKubernetesHosting();

    // Use Redis for clustering & persistence
    var redisConnectionString = $"{Environment.GetEnvironmentVariable("REDIS")}:6379";
    siloBuilder.UseRedisClustering(options => options.ConnectionString = redisConnectionString);
    siloBuilder.AddRedisGrainStorage("definitions", options => options.ConnectionString = redisConnectionString);
});


var app = builder.Build();

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

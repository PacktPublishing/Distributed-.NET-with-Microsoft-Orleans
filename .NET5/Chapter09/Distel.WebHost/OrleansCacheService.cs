using Distel.Grains.Interfaces;
using Distel.WebHost;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Distel.WebHost
{
    public class OrleansCacheService : IDistributedCache
    {
        private readonly ILogger<OrleansCacheService> logger;
        private readonly IClusterClient clusterClient;

        public OrleansCacheService(ILogger<OrleansCacheService> logger, IClusterClient clusterClient)
        {
            this.logger = logger;
            this.clusterClient = clusterClient;
        }
        public byte[] Get(string key)
        {
            var grain = this.clusterClient.GetGrain<ICacheGrain<byte[]>>(key);
            var result = grain.GetAsync().GetAwaiter().GetResult();
            return result.Value;
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default)
        {
            var grain = this.clusterClient.GetGrain<ICacheGrain<byte[]>>(key);
            var result = await grain.GetAsync();
            return result.Value;
        }

        public void Refresh(string key)
        {
            throw new NotImplementedException();
        }

        public Task RefreshAsync(string key, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            var grain = this.clusterClient.GetGrain<ICacheGrain<byte[]>>(key);
            grain.SetAsync(new Orleans.Concurrency.Immutable<byte[]>(value)).GetAwaiter().GetResult();
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            var grain = this.clusterClient.GetGrain<ICacheGrain<byte[]>>(key);
            await grain.SetAsync(new Orleans.Concurrency.Immutable<byte[]>(value));
        }
    }
}


namespace Microsoft.Extensions.DependencyInjection
{
    public static class OrleansCacheExtensions
    {
        /// <summary>
        /// Configures and registers the OrleansDistributedCacheService. You must also register an Orleans IClusterClient service.
        /// </summary>
        public static IServiceCollection AddOrleansDistributedCache(this IServiceCollection services)
        {
            services.AddOptions();
            services.AddSingleton<IDistributedCache, OrleansCacheService>();
            return services;
        }
    }
}
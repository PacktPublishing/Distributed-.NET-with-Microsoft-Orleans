using Distel.Grains.Interfaces;
using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using System.Threading.Tasks;

namespace Distel.Grains
{
    [StorageProvider]
    public class CacheGrain<T> : Grain<Immutable<T>>, ICacheGrain<T>
    {
        public Task<Immutable<T>> GetAsync()  => Task.FromResult(State);

        public async Task SetAsync(Immutable<T> value)
        {
            State = value;
            await base.WriteStateAsync();
        }
    }
}

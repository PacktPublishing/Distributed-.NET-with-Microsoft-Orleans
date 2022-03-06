using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;

namespace Distel.Grains.Interfaces
{
    public interface ICacheGrain<T> : IGrainWithStringKey
    {
        Task SetAsync(Immutable<T> value);
        Task<Immutable<T>> GetAsync();
    }
}

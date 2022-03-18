using Orleans;
using Orleans.Concurrency;

namespace Distel.Grains.Interfaces
{
    public interface IAsset : IGrainWithStringKey
    {
        [AlwaysInterleave]
        Task<TimeSpan> GetTimeToService();
    }
}

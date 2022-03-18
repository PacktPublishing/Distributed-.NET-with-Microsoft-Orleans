using Distel.DataTier.Abstractions.Models;
using Orleans;

namespace Distel.Grains.Interfaces
{
    public interface IUserGrain : IGrainWithStringKey
    {
        Task<TravelHistory> GetTravelHistory();
    }
}


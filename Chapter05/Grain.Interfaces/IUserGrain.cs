using Distel.DataTier.Abstractions.Models;
using Distel.Grains.Interfaces.Models;
using Orleans;
using System.Threading.Tasks;

namespace Distel.Grains.Interfaces
{
    public interface IUserGrain : IGrainWithStringKey
    {
        Task<TravelHistory> GetTravelHistory();
    }
}


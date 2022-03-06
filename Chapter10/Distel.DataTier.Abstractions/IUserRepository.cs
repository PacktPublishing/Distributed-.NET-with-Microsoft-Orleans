using Distel.DataTier.Abstractions.Models;
using System.Threading.Tasks;

namespace Distel.DataTier.Abstractions
{
    public interface IUserRepository
    {
        Task<TravelHistory> GetTravelHistoryAsync(string userId);
    }
}

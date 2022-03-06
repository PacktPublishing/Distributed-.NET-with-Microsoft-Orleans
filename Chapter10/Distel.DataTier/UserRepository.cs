using Distel.DataTier.Abstractions;
using Distel.DataTier.Abstractions.Models;
using System.Threading.Tasks;

namespace Distel.DataTier
{
    public class UserRepository : IUserRepository
    {
        public Task<TravelHistory> GetTravelHistoryAsync(string userId)
        {
            return Task.FromResult(new TravelHistory());
        }
    }
}

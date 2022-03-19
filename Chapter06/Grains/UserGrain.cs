using Distel.DataTier.Abstractions;
using Distel.DataTier.Abstractions.Models;
using Distel.Grains.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using System.Threading.Tasks;

namespace Distel.Grains
{
    public class UserGrain : Grain, IUserGrain
    {
        private readonly ILogger logger;
        private readonly IUserRepository userRepository;

        public UserGrain(ILogger<UserGrain> logger, IUserRepository userRepository)
        {
            this.logger = logger;
            this.userRepository = userRepository;
        }

        public async Task<TravelHistory> GetTravelHistory()
        {
            var userId = this.GetPrimaryKeyString();
            var history = await userRepository.GetTravelHistoryAsync(userId);
            return history;
        }
    }
}

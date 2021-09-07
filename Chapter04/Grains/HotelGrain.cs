using Distel.Grains.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Distel.Grains
{
    public class HotelGrain : Grain, IHotelGrain
    {
        private readonly ILogger logger;

        public HotelGrain(ILogger<HotelGrain> logger)
        {
            this.logger = logger;
        }

        public Task<string> GetKey()
        {
            return Task.FromResult(this.GetPrimaryKeyString());
        }

        public Task<decimal> ComputeDue(string guestName)
        {
            //TODO: Add code to compute the due.
            return Task.FromResult(100.00M);
        }

        public async Task OnboardFromOtherHotel(IHotelGrain fromHotel, string guestName)
        {
            logger.LogInformation($"Fetching the due from previous hotel for {guestName}");
            await fromHotel.ComputeDue(guestName);
            // TODO: Add code to onboard a guest
            logger.LogInformation($"Onbarded the guest from other hotel {guestName}");
        }

        public Task<string> WelcomeGreetingAsync(string guestName)
        {
            logger.LogInformation($"\n WelcomeGreetingAsync message received: greeting = '{guestName}'");
            return Task.FromResult($"Dear {guestName}, We welcome you to Distel and hope you enjoy a comfortable stay at our hotel. ");
        }
    }
}

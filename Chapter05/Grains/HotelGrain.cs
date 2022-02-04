using Distel.Grains.Interfaces;
using Distel.Grains.Interfaces.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Distel.Grains
{
    public class HotelGrain : Grain, IHotelGrain
    {
        private readonly ILogger logger;
        private IPersistentState<List<UserCheckIn>> checkedInGuests;
        private IPersistentState<List<Partner>> partners;

        public HotelGrain(ILogger<HotelGrain> logger, 
            [PersistentState("checkedInGuests")] 
        IPersistentState<List<UserCheckIn>> checkedInGuests,
            [PersistentState("partners", "FileShare")]
        IPersistentState<List<Partner>> partners)

        {
            this.logger = logger;
            this.checkedInGuests = checkedInGuests;
            this.partners = partners;
        }

        public override Task OnActivateAsync()
        {
            return base.OnActivateAsync();
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

        public async Task<string> CheckInGuest(UserCheckIn userCheckIn)
        {
            // TODO: Build allotment component
            checkedInGuests.State.Add(userCheckIn);
            await this.checkedInGuests.WriteStateAsync();
            
            return "";
        }

        public async Task<string> CheckOutGuest(UserCheckIn userCheckIn)
        {
            checkedInGuests.State.Remove(checkedInGuests.State.Find(e=>e.UserId == userCheckIn.UserId));
            await this.checkedInGuests.WriteStateAsync();
            return "";
        }

        public async Task AssociatePartner(Partner partner)
        {
            if (!this.partners.State.Any(e => e.Id == partner.Id))
            {
                this.partners.State.Add(partner);
                await this.partners.WriteStateAsync();
            }
        }
    }
}

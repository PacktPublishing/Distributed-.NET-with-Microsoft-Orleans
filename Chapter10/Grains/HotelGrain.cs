using Distel.Grains.Interfaces;
using Distel.Grains.Interfaces.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Distel.Grains
{
    public class HotelGrain : Grain, IHotelGrain
    {
        private readonly ILogger logger;
        private IPersistentState<List<UserCheckIn>> checkedInGuests;
        private IPersistentState<List<Partner>> partners;
        private readonly IClusterClient client;
        private IAsyncStream<AttractionNotification> stream;
        private Guid displayBoardId;
        private int hitCount = 0;
        private ICampaignGrain campaignGrain;
        private IIntermediatoryGrain intermediatoryGrain;
        public HotelGrain(ILogger<HotelGrain> logger,
            [PersistentState("checkedInGuests")]
        IPersistentState<List<UserCheckIn>> checkedInGuests,
            [PersistentState("partners")]
        IPersistentState<List<Partner>> partners,
            IClusterClient client)
        {
            this.logger = logger;
            this.checkedInGuests = checkedInGuests;
            this.partners = partners;
            this.client = client;
        }

        public override Task OnActivateAsync()
        {
            if (checkedInGuests.State == null)
            {
                checkedInGuests.State = new List<UserCheckIn>();
                partners.State = new List<Partner>();
            }
            this.displayBoardId = new Guid();
            this.stream = GetStreamProvider("attractions-stream")
                .GetStream<AttractionNotification>(displayBoardId, "AttractionEvents-NS");
            campaignGrain = this.GrainFactory.GetGrain<ICampaignGrain>("campaignKey");
            intermediatoryGrain = this.GrainFactory.GetGrain<IIntermediatoryGrain>(0);
            RegisterTimer(SendEngagedUpdate, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
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
            hitCount++;
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
            hitCount++;
            // TODO: Build allotment component
            checkedInGuests.State.Add(userCheckIn);
            await this.checkedInGuests.WriteStateAsync();
            var userGrain = this.client.GetGrain<IUserGrain>(userCheckIn.UserId);
            await userGrain.SubscribeToAttractionEventsAsync(displayBoardId, "AttractionEvents-NS");
            return "";
        }

        public async Task<string> CheckOutGuest(UserCheckIn userCheckIn)
        {
            checkedInGuests.State.Remove(checkedInGuests.State.Find(e => e.UserId == userCheckIn.UserId));
            await this.checkedInGuests.WriteStateAsync();
            return "";
        }

        public async Task AssociatePartner(Partner partner)
        {
            hitCount++;
            this.partners.State.Add(partner);
            await this.partners.WriteStateAsync();
        }

        public async Task PublishEvent(AttractionNotification attractionNotification) =>
            await this.stream.OnNextAsync(attractionNotification);

        public Task ReceiveMessage(string message)
        {
            logger.LogInformation($"\n Message received:  = '{message}'");
            return Task.CompletedTask;
        }

        public Task<int> GetAvailability()
        {
            hitCount++;
            return Task.FromResult(new Random().Next());
        }

        private async Task SendEngagedUpdate(object arg)
        {
            //await campaignGrain.ReceiveUserEngagementUpdate(this.GetPrimaryKeyString(), hitCount);
            await intermediatoryGrain.UpdateDelta(hitCount);
            hitCount = 0;
        }
    }
}

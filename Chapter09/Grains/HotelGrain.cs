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
    public class HotelGrain : Grain, IHotelGrain, IRemindable
    {
        private readonly ILogger logger;
        private IPersistentState<List<UserCheckIn>> checkedInGuests;
        private IPersistentState<List<Partner>> partners;
        private readonly IClusterClient client;
        private IAsyncStream<AttractionNotification> stream;
        private Guid displayBoardId;
        private readonly int totalRooms = 100; 
        IDisposable disposableAvailableRoomTimer;
        private readonly ObserverManager<IObserver> observerManager;
        
        private ICampaignGrain campaignGrain;
        private IIntermediatoryGrain intermediatoryGrain;
        private int hitCount = 0;


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
            
            observerManager = new ObserverManager<IObserver>(TimeSpan.FromMinutes(5), logger, "subs");

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

            disposableAvailableRoomTimer = this.RegisterTimer(this.Callback, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
            this.RegisterOrUpdateReminder("AvailableRoomCount", TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(1));

            campaignGrain = this.GrainFactory.GetGrain<ICampaignGrain>("campaignKey");
            RegisterTimer(SendEngagedUpdate, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
            
            intermediatoryGrain = this.GrainFactory.GetGrain<IIntermediatoryGrain>(0);

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
            this.partners.State.Add(partner);
            await this.partners.WriteStateAsync();
        }

        Task IRemindable.ReceiveReminder(string reminderName, TickStatus status)
        {
            Console.WriteLine("Reminder to Clean the room");
            return Task.CompletedTask;
        }


        //Task Callback(object callbackstate)
        //{
        //    if (checkOutGuests.Count > 0)
        //    {
        //        foreach (string UserId in checkOutGuests.Keys)
        //        {
        //            Console.WriteLine(UserId);
        //        }
        //    }
        //    return Task.CompletedTask;
        //}


        Task Callback(object callbackstate)
        {

            Console.WriteLine("Total available rooms {0}", totalRooms - this.checkedInGuests.State.Count);
            SendUpdateMessage("Total available rooms " + (totalRooms - this.checkedInGuests.State.Count).ToString());
            return Task.CompletedTask;
        }

        //Send message to subscribed clients
        public Task SendUpdateMessage(string message)
        {
            observerManager.Notify(s => s.ReceiveMessage(message));
            return Task.CompletedTask;
        }

        public async Task PublishEvent(AttractionNotification attractionNotification) =>
            await this.stream.OnNextAsync(attractionNotification);


        public Task ReceiveMessage(string message)
        {
            logger.LogInformation($"\n Message received:  = '{message}'");
            return Task.CompletedTask;
        }

        private async Task SendEngagedUpdate(object arg)
        {
            //await campaignGrain.ReceiveUserEngagementUpdate(this.GetPrimaryKeyString(), hitCount);
            await intermediatoryGrain.UpdateDelta(hitCount);
            hitCount = 0;
        }
    }
}

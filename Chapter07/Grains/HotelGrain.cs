using Distel.Grains.Interfaces;
using Distel.Grains.Interfaces.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Distel.Grains
{
    public class HotelGrain : Grain, IHotelGrain, IRemindable
    {
        private readonly ILogger logger;
        private IPersistentState<List<UserCheckIn>> checkedInGuests;
        private IPersistentState<List<Partner>> partners;
        private readonly int totalRooms = 100;
        IDisposable disposableAvailableRoomTimer;
        private readonly ObserverManager<IObserver> observerManager;

        public HotelGrain(ILogger<HotelGrain> logger, 
            [PersistentState("checkedInGuests")] 
        IPersistentState<List<UserCheckIn>> checkedInGuests,
            [PersistentState("partners", "FileShare")]
        IPersistentState<List<Partner>> partners)

        {
            this.logger = logger;
            this.checkedInGuests = checkedInGuests;
            this.partners = partners;
            observerManager = new ObserverManager<IObserver>(TimeSpan.FromMinutes(5), logger, "subs");
        }

        //public override Task OnActivateAsync()
        //{
        //    return base.OnActivateAsync();
        //}

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

        Task Callback(object callbackstate)
        {

            Console.WriteLine("Total available rooms {0}", totalRooms - this.checkedInGuests.State.Count);
            SendUpdateMessage("Total available rooms " + (totalRooms - this.checkedInGuests.State.Count).ToString());

            return Task.CompletedTask;
        }

        public override Task OnActivateAsync()
        {
            disposableAvailableRoomTimer = this.RegisterTimer(this.Callback, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
            this.RegisterOrUpdateReminder("AvailableRoomCount", TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(1));
            return base.OnActivateAsync();

        }

        Task IRemindable.ReceiveReminder(string reminderName, TickStatus status)
        {
            //In real world reminder can be sent to on duty person in //cleaning department 
            Console.WriteLine("Reminder to Clean the room");
            return Task.CompletedTask;
        }

        // Clients call this to subscribe.
        public Task Subscribe(IObserver observer)
        {
            observerManager.Subscribe(observer, observer);
            return Task.CompletedTask;
        }

        //Clients call this to unsubscribe 
        public Task UnSubscribe(IObserver observer)
        {
            observerManager.Unsubscribe(observer);
            return Task.CompletedTask;
        }

        public Task SendUpdateMessage(string message)
        {
            observerManager.Notify(s => s.ReceiveMessage(message));
            return Task.CompletedTask;
        }





    }
}

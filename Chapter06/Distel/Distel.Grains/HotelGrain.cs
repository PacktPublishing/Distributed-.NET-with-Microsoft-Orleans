using Distel.Grains.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.ClientObservers;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Distel.Grains
{
    public class HotelGrain : Grain, IHotelGrain, IRemindable
    {
        private readonly ILogger logger;
        private readonly IPersistentState<List<UserCheckIn>> checkedInGuests;
        Dictionary<string, DateTime> checkOutGuests;
        private readonly int totalRooms = 100;
        IDisposable disposableAvailableRoomTimer;
        private readonly ObserverManager<IObserver> observerManager;
        


        public override Task OnActivateAsync()
        {
            disposableAvailableRoomTimer = this.RegisterTimer(this.Callback, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
            this.RegisterOrUpdateReminder("AvailableRoomCount", TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(1));
            return base.OnActivateAsync();            
        }

        public HotelGrain(ILogger<HotelGrain> logger,[PersistentState("checkedInGuests")] IPersistentState<List<UserCheckIn>> checkedInGuests)
        {
            this.logger = logger;
            this.checkedInGuests = checkedInGuests;
            this.checkOutGuests = new Dictionary<string, DateTime>();
            observerManager = new ObserverManager<IObserver>(TimeSpan.FromMinutes(5), logger, "subs");

        }

        // Clients call this to subscribe.
        public Task Subscribe(IObserver observer)
        {
            observerManager.Subscribe(observer, observer);
            return Task.CompletedTask;
        }

        //Clients use this to unsubscribe and no longer receive messages.
        public Task UnSubscribe(IObserver observer)
        {
            observerManager.Unsubscribe(observer);
            return Task.CompletedTask;
        }

        //Send message to subscribed clients
        public Task SendUpdateMessage(string message)
        {
            observerManager.Notify(s => s.ReceiveMessage(message));
            return Task.CompletedTask;
        }

        public Task<string> WelcomeGreetingAsync(string guestName)
        {
            logger.LogInformation($"\n WelcomeGreetingAsync message received: greeting = '{guestName}'");
            return Task.FromResult($"Dear {guestName}, We welcome you to Distel and hope you enjoy a comfortable stay at our hotel. ");
        }

        public Task<string> GetKey()
        {
            return Task.FromResult(this.GetPrimaryKeyString());
        }

        public async Task<string> CheckInGuest(UserCheckIn userCheckIn)
        {
            checkedInGuests.State.Add(userCheckIn);
            await this.checkedInGuests.WriteStateAsync();
            return "";
        }

        public Task OnboardFromOtherHotel(IHotelGrain fromHotel, string guestName)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> ComputeDue(string guestName)
        {
            throw new NotImplementedException();
        }

        public async Task<string> CheckOutGuest(UserCheckOut userCheckOut)
        {
            this.checkOutGuests.Add(userCheckOut.UserId, userCheckOut.CheckOutDate);
            disposableAvailableRoomTimer.Dispose();
            return "";
        }

        Task IRemindable.ReceiveReminder(string reminderName, TickStatus status)
        {
            //In real world scenario, reminder can be sent to on duty person in cleaning department 
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



    }

}

using Orleans;
using System;
using System.Threading.Tasks;

namespace Distel.Grains.Interfaces
{
    public interface IHotelGrain : IGrainWithStringKey
    {
        Task<string> WelcomeGreetingAsync(string guestName);
        Task<string> GetKey();
        Task OnboardFromOtherHotel(IHotelGrain fromHotel, string guestName);
        Task<decimal> ComputeDue(string guestName);
        Task<string> CheckInGuest(UserCheckIn userCheckIn);
        Task<string> CheckOutGuest(UserCheckOut userCheckOut);
        public Task Subscribe(IObserver observer);
        public Task UnSubscribe(IObserver observer);
    }

}

using Distel.Grains.Interfaces.Models;
using Orleans;
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
        Task<string> CheckOutGuest(UserCheckIn userCheckIn);
        Task AssociatePartner(Partner partner);
        Task PublishEvent(AttractionNotification attractionNotification);
        Task ReceiveMessage(string message);
        Task<int> GetAvailability();
    }
}


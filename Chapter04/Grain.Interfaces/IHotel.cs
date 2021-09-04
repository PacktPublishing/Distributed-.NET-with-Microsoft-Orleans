using Orleans;
using System.Threading.Tasks;

namespace Distel.Grains.Interfaces
{
    public interface IHotel : IGrainWithStringKey
    {
        Task<string> WelcomeGreeting(string guestName);
        Task<string> GetKey();

        Task OnboardFromOtherHotel(IHotel fromHotel, string guestName);
        Task<decimal> ComputeDue(string guestName);
    }
}


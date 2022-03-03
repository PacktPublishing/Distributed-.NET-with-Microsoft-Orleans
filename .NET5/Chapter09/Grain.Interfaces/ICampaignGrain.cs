using Orleans;
using System.Threading.Tasks;

namespace Distel.Grains.Interfaces
{
    public interface ICampaignGrain : IGrainWithStringKey
    {
        Task ReceiveUserEngagementUpdate(string hotelGrianId, int hitCount);
        Task ReceiveUserEngagementUpdate(int hitCount);
    }
}
